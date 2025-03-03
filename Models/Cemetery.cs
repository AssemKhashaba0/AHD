using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Cemetery
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "يجب أن يكون الاسم بين 3 و 100 حرف")]
        public string Name { get; set; }

        [Required(ErrorMessage = "يجب تحديد المدينة")]
        public int CityId { get; set; }

       
        [ValidateNever]
        public City City { get; set; }
    }
}


