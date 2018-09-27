using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class IntentDto
    {
        public string id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string description { get; set; }
        public bool is_activated { get; set; }
        public int position { get; set; }
        public int expressions_count { get; set; }
        public int suggestions_count { get; set; }
    }
}
