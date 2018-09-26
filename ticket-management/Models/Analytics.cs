using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class Analytics
    {
        [Key]
        public int Id { get ; set; }
        public DateTime Date { get; set ; }
        public string Avgresolutiontime { get ; set ; }
        public double Csatscore { get ; set ; }
        public int Customerid { get ; set ; }
    }
}
