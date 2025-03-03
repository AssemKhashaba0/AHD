using Microsoft.AspNetCore.Mvc;
using DataAccess.Repository.IRepository;
using Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace AHD.Controllers
{
    public class SelectionController : Controller
    {
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<Mosque> _mosqueRepository;
        private readonly IRepository<Cemetery> _cemeteryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Cart> _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public SelectionController(
            IRepository<City> cityRepository,
            IRepository<Mosque> mosqueRepository,
            IRepository<Cemetery> cemeteryRepository,
            IRepository<Product> productRepository,
            IRepository<Cart> cartRepository,
            UserManager<ApplicationUser> userManager)
        {
            _cityRepository = cityRepository;
            _mosqueRepository = mosqueRepository;
            _cemeteryRepository = cemeteryRepository;
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        public IActionResult ChooseType()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }
            return View();
        }

        public IActionResult SelectLocation(int? cityId, string selectionType)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }
            var cities = _cityRepository.GetAll().ToList();
            var mosques = cityId.HasValue
                ? _mosqueRepository.Get(expression: m => m.CityId == cityId).ToList()
                : new List<Mosque>();
            var cemeteries = cityId.HasValue
                ? _cemeteryRepository.Get(expression: c => c.CityId == cityId).ToList()
                : new List<Cemetery>();

            ViewBag.Cities = cities;
            ViewBag.SelectedCityId = cityId;
            ViewBag.SelectionType = selectionType;
            ViewBag.Mosques = mosques;
            ViewBag.Cemeteries = cemeteries;

            if (cityId.HasValue && selectionType == "mosque" && !mosques.Any())
                ViewBag.Message = "لا يوجد مساجد حاليًا في هذه المدينة.";
            else if (cityId.HasValue && selectionType == "cemetery" && !cemeteries.Any())
                ViewBag.Message = "لا يوجد مقابر حاليًا في هذه المدينة.";

            return View();
        }

        public IActionResult SelectProducts(int? mosqueId, int? cemeteryId, int cityId)
        {
            var products = _productRepository.GetAll().ToList();

            Mosque mosque = null;
            Cemetery cemetery = null;
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }
            if (mosqueId.HasValue)
            {
                mosque = _mosqueRepository.GetFirstOrDefault(m => m.Id == mosqueId.Value);
            }
            else if (cemeteryId.HasValue)
            {
                cemetery = _cemeteryRepository.GetFirstOrDefault(c => c.Id == cemeteryId.Value);
            }

            ViewBag.Products = products;
            ViewBag.CityId = cityId;
            ViewBag.Mosque = mosque;
            ViewBag.Cemetery = cemetery;
            return View();
        }

        public IActionResult Cart()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login");
            }

            var cartItems = _cartRepository.Get(
                includeProp: new Expression<System.Func<Cart, object>>[]
                {
                    c => c.Product,
                    c => c.City,
                    c => c.Mosque,
                    c => c.Cemetery
                },
                expression: c => c.ApplicationUserId == userId
            ).ToList();

            ViewBag.TotalPrice = cartItems.Sum(c => c.Product?.Price * c.Count ?? 0);
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int cityId, int? mosqueId, int? cemeteryId, int count)
        {
            try
            {
                var product = _productRepository.GetById(productId);
                if (product == null)
                {
                    TempData["error"] = "المنتج غير موجود.";
                    return RedirectToAction("Index");
                }

                // التحقق من أن الكمية المطلوبة لا تتجاوز الكمية المتاحة
                if (count > product.Stock)
                {
                    TempData["error"] = "الكمية المطلوبة غير متاحة في المخزن.";
                    return RedirectToAction("Index");
                }

                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["error"] = "يجب تسجيل الدخول لإضافة المنتجات إلى السلة.";
                    return RedirectToAction("Index");
                }

                var cartItem = _cartRepository.GetFirstOrDefault(
                    c => c.ApplicationUserId == userId && c.ProductId == productId && c.CityId == cityId
                );

                if (cartItem == null)
                {
                    // إنشاء عنصر جديد في السلة
                    cartItem = new Cart
                    {
                        ApplicationUserId = userId,
                        ProductId = productId,
                        CityId = cityId,
                        MosqueId = mosqueId,
                        CemeteryId = cemeteryId,
                        Count = count
                    };
                    _cartRepository.Create(cartItem);
                }
                else
                {
                    // تحديث الكمية إذا كان العنصر موجودًا بالفعل
                    cartItem.Count += count;
                    _cartRepository.Edit(cartItem);
                }

                _cartRepository.Commit();
                TempData["success"] = "تمت إضافة المنتج إلى السلة بنجاح!";
                return RedirectToAction("Cart");
            }
            catch (Exception ex)
            {
                TempData["error"] = $"حدث خطأ أثناء إضافة المنتج إلى السلة: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult UpdateCart(int cartId, int count)
        {
            var cartItem = _cartRepository.GetFirstOrDefault(c => c.Id == cartId);
            if (cartItem != null)
            {
                cartItem.Count = count > 0 ? count : 1;
                _cartRepository.Edit(cartItem);
                _cartRepository.Commit();
            }
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int cartId)
        {
            var cartItem = _cartRepository.GetFirstOrDefault(c => c.Id == cartId);
            if (cartItem != null)
            {
                _cartRepository.Delete(cartItem);
                _cartRepository.Commit();
            }
            return RedirectToAction("Cart");
        }

        public IActionResult Checkout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            return View();
        }
    }
}