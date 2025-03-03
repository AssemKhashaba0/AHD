using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [StringLength(100, ErrorMessage = "يجب ألا يتجاوز الاسم 100 حرف")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "يجب ألا يتجاوز الوصف 500 حرف")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "يجب تحديد صورة المنتج")]
        [ValidateNever]

        public string ImageUrl { get; set; } // سيتم تخزين اسم الصورة فقط

        [Required(ErrorMessage = "يجب تحديد المخزون")]
        [Range(0, int.MaxValue, ErrorMessage = "المخزون لا يمكن أن يكون سالبًا")]
        public int Stock { get; set; }
        [Required(ErrorMessage = "يجب تحديد السعر")]
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن يكون السعر أكبر من 0")]
        public decimal Price { get; set; }

    }
}
