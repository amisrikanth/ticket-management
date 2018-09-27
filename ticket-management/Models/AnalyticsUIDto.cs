using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    public class AnalyticsUIDto
    {
        public List<AnalyticsCountDto> Analyticscount { get ; set ; }

        public List<AvgResolutionTime> Avgresolutiontime { get ; set ; }
 public List<AnalyticsCsatDto> Analyticscsat { get ; set ; }
    }
    public class AnalyticsCsatDto
    {
        public DateTime Date { get ; set ; }
        public double Csatscore { get ; set ; }
    }
    public class AnalyticsCountDto
    {
        public string Tickettype { get ; set ; }
        public long Count { get ; set ; }
    }

    public class AvgResolutionTime
    {
        public string intent { get; set; }
        public double avgresolutiontime { get; set; }

    }
}
