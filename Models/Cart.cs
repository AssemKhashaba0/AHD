using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Cart
    {
        public int Id { get; set; } 

        public int ProductId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public int CityId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(CityId))]
        public City City { get; set; }

        public string ApplicationUserId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Count { get; set; }

        public int? MosqueId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(MosqueId))]
        public Mosque Mosque { get; set; }

        public int? CemeteryId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(CemeteryId))]
        public Cemetery Cemetery { get; set; }
    }
}
