using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("Mosque")]
        public int? MosqueId { get; set; }
        public Mosque Mosque { get; set; }

        [ForeignKey("Cemetery")]
        public int? CemeteryId { get; set; }
        public Cemetery Cemetery { get; set; }

        [Required]
        public OrderStatusEnum OrderStatus { get; set; }

        [Required]
        public OrderTypeEnum OrderType { get; set; }

        [Required]
        public PaymentMethodEnum PaymentMethod { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public ICollection<Payment> Payments { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // موصل الطلبات
        public string? DeliveryManId { get; set; }

        [ForeignKey("DeliveryManId")]
        [ValidateNever]

        public ApplicationUser DeliveryMan { get; set; }
        [ValidateNever]

        public int? DeliveryLocationId { get; set; } // أو يمكن أن تكون من نوع DeliveryLocation
        [ValidateNever]

        public DeliveryLocation? DeliveryLocation { get; set; }
    }

    public enum OrderStatusEnum
    {
        New,
        InProgress,
        InTheWay,
        Completed,
        Canceled
    }

    public enum OrderTypeEnum
    {
        OneTime,
        Daily,
        Weekly,
        Monthly,
        Custom
    }

    public enum PaymentMethodEnum
    {
        Cash,
        OnDelivery,
        PaymentMade
    }
}
