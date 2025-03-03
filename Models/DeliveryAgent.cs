using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DeliveryAgent
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? AssignedOrderId { get; set; }
        public string Status { get; set; } // ENUM(in_progress, completed, canceled)
        public DateTime UpdatedAt { get; set; }

        public ApplicationUser User { get; set; }
        public Order AssignedOrder { get; set; }
    }

}
