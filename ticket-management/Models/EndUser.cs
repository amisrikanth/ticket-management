using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class EndUser
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Organisation Organization { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long UpdatedBy { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImgUrl { get; set; }
    }
}
