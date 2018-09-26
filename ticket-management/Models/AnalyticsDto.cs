using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class AnalyticsDto
    {

        public int Customerid { get ; set ; }
        public double Csatscore { get ; set ; }
        public string Avgresolutiontime { get ; set ; }
    }
}
