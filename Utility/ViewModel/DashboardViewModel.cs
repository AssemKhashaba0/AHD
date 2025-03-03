using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ViewModel
{
    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int NewOrders { get; set; }
        public int InProgressOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CanceledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Order> Last24HoursOrders { get; set; }
        public List<Order> RecentOrders { get; set; }
        public List<Payment> RecentPayments { get; set; }
    }
}
