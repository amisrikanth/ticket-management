using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class TopAgentsDto
    {
        public int NumberOfTicketsResolved { get; set ; }
        public string Name { get; set ; }
        public string ProfileImageUrl { get ; set ; }
        public string DepartmentName { get; set ; }
    }
}
