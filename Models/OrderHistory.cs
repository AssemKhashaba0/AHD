using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class OrderHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [Required]
        public string UpdatedBy { get; set; } 

        [ForeignKey("UpdatedBy")]
        public ApplicationUser User { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public OrderStatusEnum NewStatus { get; set; }
    }
}
