using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticket_management.Models
{
    //user id
    public class TicketCount
    {
        long total;
        long open;
        long closed;
        long due;

        public long Total { get => total; set => total = value; }
        public long Open { get => open; set => open = value; }
        public long Closed { get => closed; set => closed = value; }
        public long Due { get => due; set => due = value; }
    }
}
