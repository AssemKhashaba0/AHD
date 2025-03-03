using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DeliveryLocation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "المدينة مطلوبة")]
        [MaxLength(100, ErrorMessage = "يجب ألا تتجاوز المدينة 100 حرف")]
        public string City { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        [MaxLength(250, ErrorMessage = "يجب ألا يتجاوز العنوان 250 حرف")]
        public string Address { get; set; }

        [Required(ErrorMessage = "الشارع مطلوب")]
        [MaxLength(150, ErrorMessage = "يجب ألا يتجاوز الشارع 150 حرف")]
        public string Street { get; set; }

        [MaxLength(500, ErrorMessage = "يجب ألا تتجاوز التفاصيل الإضافية 500 حرف")]
        public string? AdditionalDetails { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        public string PhoneNumber { get; set; }

        [Url(ErrorMessage = "يجب إدخال رابط صالح")]
        public string? LocationLink { get; set; }

        public string UserId { get; set; } // إضافة معرف المستخدم
    }
}
