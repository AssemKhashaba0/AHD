using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Models;
using Stripe.Checkout;
using System.Linq.Expressions;
using Utility;

namespace AHD.Controllers
{

    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Mosque> _mosqueRepository;
        private readonly IRepository<Cemetery> _cemeteryRepository;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<DeliveryLocation> deliveryLocationRepository;
        private readonly IEmailSender emailSender;


        public OrderController(
            UserManager<ApplicationUser> userManager,
            IRepository<Order> orderRepository,
            IRepository<Mosque> mosqueRepository,
            IRepository<Cemetery> cemeteryRepository,
             IEmailSender emailSender,
             IRepository<Product> productRepository,
             IRepository<Payment> paymentRepository,
                         IRepository<DeliveryLocation> deliveryLocationRepository,

            IRepository<Cart> cartRepository) // تعديل اسم المتغير
        {
            this.emailSender = emailSender;
            _userManager = userManager;
            _orderRepository = orderRepository;
            _mosqueRepository = mosqueRepository;
            _cemeteryRepository = cemeteryRepository;
            _cartRepository = cartRepository; // تصحيح التخزين
            _productRepository = productRepository;
            this._paymentRepository = paymentRepository;
            this.deliveryLocationRepository = deliveryLocationRepository;
        }
        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

        public async Task<IActionResult> PaymentDetails(string paymentStatus = null, string paymentMethod = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                // تحسين التصفيات
                var payments = _paymentRepository.Get(
                    includeProp: new Expression<Func<Payment, object>>[] { p => p.Order, p => p.User },
                    expression: p =>
                        (string.IsNullOrEmpty(paymentStatus) || p.PaymentStatus.ToLower() == paymentStatus.ToLower()) &&
                        (string.IsNullOrEmpty(paymentMethod) || p.PaymentMethod.ToLower() == paymentMethod.ToLower()) &&
                        (!fromDate.HasValue || p.TransactionDate >= fromDate.Value) &&
                        (!toDate.HasValue || p.TransactionDate <= toDate.Value)
                ).ToList();

                return View(payments);
            }
            catch (Exception ex)
            {
                TempData["error"] = "حدث خطأ أثناء جلب بيانات الدفوعات.";
                Console.WriteLine($"Error in PaymentDetails: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public IActionResult Checkout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            var userId = _userManager.GetUserId(User);
            var cartItems = _cartRepository.Get(
                expression: c => c.ApplicationUserId == userId,
                includeProp: new Expression<Func<Cart, object>>[] { c => c.Product }
            ).ToList();

            ViewBag.CartItems = cartItems; // إرسال عناصر السلة إلى View
            return View();
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Checkout(string PaymentMethod, int? MosqueId, int? CemeteryId, DeliveryLocation deliveryLocation)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    TempData["error"] = "يجب تسجيل الدخول لإتمام الطلب.";
                    return RedirectToAction("Cart", "selection");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "المستخدم غير موجود.";
                    return RedirectToAction("Cart", "selection");
                }

                if (deliveryLocation == null || string.IsNullOrEmpty(deliveryLocation.Address))
                {
                    TempData["error"] = "يجب إدخال عنوان التوصيل قبل المتابعة.";
                    return RedirectToAction("Checkout");
                }

                var cartItems = _cartRepository.Get(
                    expression: c => c.ApplicationUserId == user.Id,
                    includeProp: new Expression<Func<Cart, object>>[] { c => c.Product }
                ).ToList();

                if (!cartItems.Any())
                {
                    TempData["error"] = "السلة فارغة.";
                    return RedirectToAction("Cart", "selection");
                }

                decimal totalPrice = cartItems.Sum(item => item.Product.Price * item.Count);

                // حفظ العنوان في قاعدة البيانات
                deliveryLocation.UserId = user.Id;
                deliveryLocationRepository.Create(deliveryLocation);
                deliveryLocationRepository.Commit(); // تأكد من حفظ العنوان

                if (PaymentMethod == "Cash")
                {
                    // إنشاء الطلب قبل إنشاء جلسة الدفع
                    var order = new Order
                    {
                        UserId = user.Id,
                        MosqueId = MosqueId,
                        CemeteryId = CemeteryId,
                        OrderStatus = OrderStatusEnum.New,
                        OrderType = OrderTypeEnum.OneTime,
                        PaymentMethod = PaymentMethodEnum.Cash,
                        TotalPrice = totalPrice,
                        CreatedAt = DateTime.UtcNow,
                        DeliveryLocation = deliveryLocation
                    };

                    _orderRepository.Create(order);
                    _orderRepository.Commit(); // تأكد من حفظ الطلب

                    // إنشاء سجل الدفع
                    var payment = new Payment
                    {
                        OrderId = order.Id, // تأكد من تعيين OrderId
                        UserId = user.Id,
                        Amount = totalPrice,
                        PaymentStatus = "pending", // حالة الدفع (قيد الانتظار)
                        PaymentMethod = PaymentMethod, // طريقة الدفع
                        TransactionDate = DateTime.UtcNow // تاريخ المعاملة
                    };

                    _paymentRepository.Create(payment);
                    _paymentRepository.Commit(); // تأكد من حفظ الدفع

                    // إنشاء جلسة دفع باستخدام Stripe
                    var domain = $"{Request.Scheme}://{Request.Host}";
                    var options = new SessionCreateOptions
                    {
                        PaymentMethodTypes = new List<string> { "card" },
                        LineItems = cartItems.Select(item => new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions { Name = item.Product.Name },
                                UnitAmount = (long)(item.Product.Price * 100),
                            },
                            Quantity = item.Count,
                        }).ToList(),
                        Mode = "payment",
                        SuccessUrl = $"{domain}/Order/PaymentSuccess?orderId={order.Id}",
                        CancelUrl = $"{domain}/Order/PaymentCancel?orderId={order.Id}",
                    };

                    var service = new SessionService();
                    var session = service.Create(options);

                    return Redirect(session.Url);
                }
                else if (PaymentMethod == "OnDelivery")
                {
                    // إنشاء الطلب مباشرة للدفع عند الاستلام
                    var order = new Order
                    {
                        UserId = user.Id,
                        MosqueId = MosqueId,
                        CemeteryId = CemeteryId,
                        OrderStatus = OrderStatusEnum.New,
                        OrderType = OrderTypeEnum.OneTime,
                        PaymentMethod = PaymentMethodEnum.OnDelivery,
                        TotalPrice = totalPrice,
                        CreatedAt = DateTime.UtcNow,
                        DeliveryLocation = deliveryLocation
                    };

                    _orderRepository.Create(order);
                    _orderRepository.Commit(); // تأكد من حفظ الطلب

                    // إنشاء سجل الدفع
                    var payment = new Payment
                    {
                        OrderId = order.Id, // تأكد من تعيين OrderId
                        UserId = user.Id,
                        Amount = totalPrice,
                        PaymentStatus = "successful", // حالة الدفع
                        PaymentMethod = PaymentMethod, // طريقة الدفع
                        TransactionDate = DateTime.UtcNow // تاريخ المعاملة
                    };

                    _paymentRepository.Create(payment);
                    _paymentRepository.Commit(); // تأكد من حفظ الدفع

                    // تحديث المخزون وحذف العناصر من السلة
                    foreach (var item in cartItems)
                    {
                        var product = _productRepository.GetById(item.Product.Id);
                        if (product != null && product.Stock >= item.Count)
                        {
                            product.Stock -= item.Count;
                            _productRepository.Edit(product);
                        }
                        _cartRepository.Delete(item);
                    }
                    _productRepository.Commit();
                    _cartRepository.Commit();

                    TempData["success"] = "تم إنشاء الطلب بنجاح! سيتم الدفع عند الاستلام.";
                    return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
                }
                else
                {
                    TempData["error"] = "طريقة الدفع غير مدعومة.";
                    return RedirectToAction("Cart", "selection");
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"حدث خطأ أثناء معالجة الطلب: {ex.Message}";
                Console.WriteLine($"Error in Checkout: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\n{ex.StackTrace}");
                return RedirectToAction("Cart", "selection");
            }
        }
        [HttpGet]
        public async Task<IActionResult> PaymentSuccess(int orderId)
        {
            try
            {
                var order = _orderRepository.GetById(orderId);
                if (order == null)
                {
                    TempData["error"] = "الطلب غير موجود.";
                    return RedirectToAction("Cart", "selection");
                }

                // تحديث حالة الدفع إلى "successful"
                var payment = _paymentRepository.GetFirstOrDefault(p => p.OrderId == orderId);
                if (payment != null)
                {
                    payment.PaymentStatus = "successful";
                    _paymentRepository.Edit(payment);
                    _paymentRepository.Commit();
                }

                TempData["success"] = "تم الدفع بنجاح وتم إنشاء الطلب!";
                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }
            catch (Exception ex)
            {
                TempData["error"] = $"حدث خطأ أثناء معالجة الدفع: {ex.Message}";
                Console.WriteLine($"Error in PaymentSuccess: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\n{ex.StackTrace}");
                return RedirectToAction("Cart", "selection");
            }
        }

        [HttpGet]
        public IActionResult PaymentCancel(int orderId)
        {
            try
            {
                var order = _orderRepository.GetById(orderId);
                if (order == null)
                {
                    TempData["error"] = "الطلب غير موجود.";
                    return RedirectToAction("Cart", "selection");
                }

                // تحديث حالة الدفع إلى "failed"
                var payment = _paymentRepository.GetFirstOrDefault(p => p.OrderId == orderId);
                if (payment != null)
                {
                    payment.PaymentStatus = "failed";
                    _paymentRepository.Edit(payment);
                    _paymentRepository.Commit();
                }

                TempData["error"] = "تم إلغاء الدفع، لم يتم إنشاء الطلب.";
                return RedirectToAction("Cart", "selection");
            }
            catch (Exception ex)
            {
                TempData["error"] = $"حدث خطأ أثناء معالجة الدفع: {ex.Message}";
                Console.WriteLine($"Error in PaymentCancel: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\n{ex.StackTrace}");
                return RedirectToAction("Cart", "selection");
            }
        }



        public IActionResult OrderConfirmation(int orderId)
        {
            var order = _orderRepository.Get(
                includeProp: new Expression<Func<Order, object>>[] { o => o.DeliveryLocation },
                expression: o => o.Id == orderId
            ).FirstOrDefault();

            if (order == null)
            {
                TempData["error"] = "الطلب غير موجود.";
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                
            }

            var order = _orderRepository.Get(
                expression: o => o.Id == id && o.UserId == user.Id,
                includeProp: new Expression<Func<Order, object>>[]
                {
            o => o.Mosque,
            o => o.Cemetery
                }
            ).FirstOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]
        [HttpGet]
        public IActionResult AdminOrders()
        {
            var orders = _orderRepository.Get(
                includeProp: new Expression<Func<Order, object>>[]
                {
            o => o.User,
            o => o.Mosque,
            o => o.Cemetery,
            o => o.DeliveryLocation // تضمين تفاصيل العنوان
                }
            ).ToList();

            return View(orders);
        }
        [HttpPost]
        [Authorize(Roles = $"{SD.AdminRoles},{SD.DAgentRoles}")]

        public IActionResult UpdateOrderStatus(int orderId, OrderStatusEnum status)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                TempData["error"] = "يجب تسجيل الدخول.";
                 return Redirect("/Identity/Account/Login");
            }

            var order = _orderRepository.GetFirstOrDefault(o => o.Id == orderId && o.DeliveryManId == user.Id);
            if (order == null)
            {
                TempData["error"] = "الطلب غير موجود أو لا تملك صلاحية لتعديله.";
                return RedirectToAction("DeliveryManOrders");
            }

            // تحديث حالة الطلب
            order.OrderStatus = status;
            _orderRepository.Edit(order);
            _orderRepository.Commit();

            TempData["success"] = "تم تحديث حالة الطلب بنجاح.";
            return RedirectToAction("DeliveryManOrders");
        }


      

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null || order.OrderStatus != OrderStatusEnum.New)
            {
                TempData["error"] = "لا يمكن إلغاء الطلب بعد معالجته أو إذا لم يكن موجودًا.";
                return RedirectToAction("UserOrders");
            }

            var user = await _userManager.FindByIdAsync(order.UserId);
            if (user != null)
            {
                var subject = "تم إلغاء الطلب";
                var message = $"مرحبًا {user.UserName},<br/><br/>تم إلغاء الطلب رقم {order.Id}. إذا كنت بحاجة إلى مساعدة، يرجى التواصل معنا.<br/><br/>شكرًا لاستخدامك خدماتنا.";
                await emailSender.SendEmailAsync(user.Email, subject, message);
            }

            _orderRepository.Delete(order);
            _orderRepository.Commit();

            TempData["success"] = "تم إلغاء الطلب بنجاح.";
            return RedirectToAction("UserOrders");
        }
        public async Task<IActionResult> MyOrders(OrderStatusEnum? statusFilter)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Redirect("/Identity/Account/Login");
            }

            var ordersQuery = _orderRepository.Get(
                expression: o => o.UserId == user.Id,
                includeProp: new Expression<Func<Order, object>>[]
                {
            o => o.Mosque,
            o => o.Cemetery,
                }
            );

            if (statusFilter.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderStatus == statusFilter.Value);
            }

            var orders = ordersQuery.ToList();

            if (!orders.Any())
            {
                ViewBag.NoOrdersMessage = "لم يتم طلب أي طلبات بعد.";
            }

            return View(orders);
        }

        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

        [HttpGet]
        public IActionResult Last24HoursOrders(int page = 1, int pageSize = 10)
        {
            var orders = _orderRepository.Get(
                expression: o => o.CreatedAt >= DateTime.UtcNow.AddHours(-24),
                includeProp: new Expression<Func<Order, object>>[] { o => o.User, o => o.Mosque, o => o.Cemetery }
            )
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            return View("Last24HoursOrders", orders);
        }

        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

        [HttpGet]
        public IActionResult NewOrders(int page = 1, int pageSize = 10)
        {
            var orders = _orderRepository.Get(
                expression: o => o.OrderStatus == OrderStatusEnum.New,
                includeProp: new Expression<Func<Order, object>>[] { o => o.User, o => o.Mosque, o => o.Cemetery }
            )
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            return View("NewOrders", orders);
        }

        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

        [HttpGet]
        public IActionResult ProcessingOrders(int page = 1, int pageSize = 10)
        {
            var orders = _orderRepository.Get(
                expression: o => o.OrderStatus == OrderStatusEnum.InProgress,
                includeProp: new Expression<Func<Order, object>>[] { o => o.User, o => o.Mosque, o => o.Cemetery }
            )
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            return View("ProcessingOrders", orders);
        }

        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

        [HttpGet]
        public IActionResult OnTheWayOrders(int page = 1, int pageSize = 10)
        {
            var orders = _orderRepository.Get(
                expression: o => o.OrderStatus == OrderStatusEnum.InTheWay,
                includeProp: new Expression<Func<Order, object>>[] { o => o.User, o => o.Mosque, o => o.Cemetery }
            )
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            return View("OnTheWayOrders", orders);
        }
        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

        [HttpGet]
        public IActionResult DeliveredOrders(int page = 1, int pageSize = 10)
        {
            var orders = _orderRepository.Get(
                expression: o => o.OrderStatus == OrderStatusEnum.Completed,
                includeProp: new Expression<Func<Order, object>>[] { o => o.User, o => o.Mosque, o => o.Cemetery }
            )
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            return View("DeliveredOrders", orders);
        }

        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

        [HttpGet]
        public IActionResult CanceledOrders(int page = 1, int pageSize = 10)
        {
            var orders = _orderRepository.Get(
                expression: o => o.OrderStatus == OrderStatusEnum.Canceled,
                includeProp: new Expression<Func<Order, object>>[] { o => o.User, o => o.Mosque, o => o.Cemetery }
            )
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            return View("CanceledOrders", orders);
        }

        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]
        [HttpGet]
        public IActionResult AssignDeliveryMan(int orderId)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null)
            {
                TempData["error"] = "الطلب غير موجود.";
                return RedirectToAction("AdminOrders");
            }

            // استرجاع المستخدمين الذين لديهم الصلاحية "DAgent"
            var deliveryAgents = _userManager.GetUsersInRoleAsync("DAgent").Result.ToList();
            ViewBag.DeliveryMen = deliveryAgents;
            ViewBag.OrderId = orderId;

            return View();
        }
        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

        [HttpPost]
        public async Task<IActionResult> AssignDeliveryMan(int orderId, string deliveryManId)
        {
            var order = _orderRepository.GetById(orderId);
            if (order == null)
            {
                TempData["error"] = "الطلب غير موجود.";
                return RedirectToAction("AdminOrders");
            }

            var deliveryMan = await _userManager.FindByIdAsync(deliveryManId);
            if (deliveryMan == null || !(await _userManager.IsInRoleAsync(deliveryMan, "DAgent")))
            {
                TempData["error"] = "موصل الطلبات غير موجود أو لا يمتلك الصلاحية المطلوبة.";
                return RedirectToAction("AssignDeliveryMan", new { orderId = orderId });
            }

            order.DeliveryManId = deliveryManId;
            _orderRepository.Edit(order);
            _orderRepository.Commit();

            TempData["success"] = "تم تعيين موصل الطلبات بنجاح.";
            return RedirectToAction("AdminOrders");
        }

        [Authorize(Roles = SD.DAgentRoles)]
        [HttpGet]
        public async Task<IActionResult> DeliveryManOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !(await _userManager.IsInRoleAsync(user, "DAgent")))
            {
                TempData["error"] = "يجب تسجيل الدخول كموصل طلبات لعرض الطلبات.";
                return RedirectToAction("Index", "Home");
            }

            var orders = _orderRepository.Get(
                expression: o => o.DeliveryManId == user.Id,
                includeProp: new Expression<Func<Order, object>>[]
                {
            o => o.User,
            o => o.Mosque,
            o => o.Cemetery,
            o => o.DeliveryLocation // تضمين تفاصيل العنوان
                }
            ).ToList();

            return View(orders);
        }

        [HttpGet]
        [Authorize(Roles = SD.DAgentRoles)]

        public async Task<IActionResult> DeliveryManOrderStatus()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !(await _userManager.IsInRoleAsync(user, "DAgent")))
            {
                TempData["error"] = "يجب تسجيل الدخول كموصل طلبات لعرض الطلبات.";
                
            }

            var ordersToDeliver = _orderRepository.Get(
                expression: o => o.DeliveryManId == user.Id && o.OrderStatus == OrderStatusEnum.InTheWay,
                includeProp: new Expression<Func<Order, object>>[] { o => o.User, o => o.Mosque, o => o.Cemetery }
            ).ToList();

            var deliveredOrders = _orderRepository.Get(
                expression: o => o.DeliveryManId == user.Id && o.OrderStatus == OrderStatusEnum.Completed,
                includeProp: new Expression<Func<Order, object>>[] { o => o.User, o => o.Mosque, o => o.Cemetery }
            ).ToList();

            ViewBag.OrdersToDeliver = ordersToDeliver;
            ViewBag.DeliveredOrders = deliveredOrders;

            return View();
        }

    }
}
