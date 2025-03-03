using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } // ENUM(successful, failed)
        public string PaymentMethod { get; set; } // ENUM(cash, on_delivery)
        public DateTime TransactionDate { get; set; }

        public Order Order { get; set; }
        public ApplicationUser User { get; set; }
    }

}
