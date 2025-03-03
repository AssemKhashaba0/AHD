using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ViewModel
{
    public class UserWithRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
    }

    public class ManageRolesViewModel
    {
        public List<UserWithRoleViewModel> Users { get; set; }
        public List<string> Roles { get; set; }
    }


}
