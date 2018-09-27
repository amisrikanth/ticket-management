using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class ChatDto
    {
        public string Description { get ; set ; }
        public string Customerhandle { get ; set ; }
        public long Userid { get ; set ; }
        public string Connectionid { get ; set ; }
    }
}
