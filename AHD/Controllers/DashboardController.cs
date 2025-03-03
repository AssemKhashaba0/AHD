using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Utility;
using Utility.ViewModel;

namespace AHD.Controllers
{
    [Authorize]
    public class DashboardController : Controller
        {
            private readonly IRepository<Order> _orderRepository;
            private readonly IRepository<Payment> _paymentRepository;

            public DashboardController(
                IRepository<Order> orderRepository,
                IRepository<Payment> paymentRepository)
            {
                _orderRepository = orderRepository;
                _paymentRepository = paymentRepository;
        }
        [HttpGet]
        public IActionResult Welcome()
        {
            return View();
        }
        [Authorize(Roles = SD.AdminRoles)]

        [HttpGet]
        public IActionResult Index()
        {
            var dashboardData = new DashboardViewModel
            {
                // إحصائيات الطلبات
                TotalOrders = _orderRepository.GetAll().Count(),
                NewOrders = _orderRepository.Get(expression: o => o.OrderStatus == OrderStatusEnum.New).Count(),
                InProgressOrders = _orderRepository.Get(expression: o => o.OrderStatus == OrderStatusEnum.InProgress).Count(),
                CompletedOrders = _orderRepository.Get(expression: o => o.OrderStatus == OrderStatusEnum.Completed).Count(),
                CanceledOrders = _orderRepository.Get(expression: o => o.OrderStatus == OrderStatusEnum.Canceled).Count(),

                // الإيرادات
                TotalRevenue = _paymentRepository.Get(expression: p => p.PaymentStatus == "successful").Sum(p => p.Amount),

                // الطلبات في آخر 24 ساعة
                Last24HoursOrders = _orderRepository
                    .Get(expression: o => o.CreatedAt >= DateTime.UtcNow.AddHours(-24))
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(10)
                    .ToList(),

                // الطلبات الحديثة
                RecentOrders = _orderRepository
                    .GetAll()
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(5)
                    .ToList(),

                // المدفوعات الحديثة
                RecentPayments = _paymentRepository
                    .GetAll()
                    .OrderByDescending(p => p.TransactionDate)
                    .Take(5)
                    .ToList()
            };

            return View(dashboardData);
        }
    
    }
}

