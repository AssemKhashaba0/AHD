using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ViewModel
{
    public class UserDetailsViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Order> Orders { get; set; }
    }
}
