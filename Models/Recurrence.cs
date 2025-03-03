using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Recurrence
    {
        public int Id { get; set; }
        public int OrderId { get; set; } 
        public string Frequency { get; set; } 
        public DateTime NextOccurrence { get; set; } 
        public Order Order { get; set; } 
    }

}



