using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Mosque
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المسجد مطلوب")]
        [StringLength(100, ErrorMessage = "يجب ألا يتجاوز الاسم 100 حرف")]
        public string Name { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(255, ErrorMessage = "يجب ألا يتجاوز العنوان 255 حرفًا")]
        public string Address { get; set; }

        [Required(ErrorMessage = "خط العرض مطلوب")]
        [Range(-90, 90, ErrorMessage = "يجب أن يكون خط العرض بين -90 و 90")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "خط الطول مطلوب")]
        [Range(-180, 180, ErrorMessage = "يجب أن يكون خط الطول بين -180 و 180")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "معرف المدينة مطلوب")]
        [ForeignKey("City")]
        public int CityId { get; set; }

        [ValidateNever]

        // العلاقة مع الكيانات الأخرى
        public City City { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
