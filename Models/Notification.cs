using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public string Status { get; set; } // ENUM(read, unread)
        public DateTime CreatedAt { get; set; }

        public ApplicationUser User { get; set; }
    }

}
