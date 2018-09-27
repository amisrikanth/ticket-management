using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ticket_management.Models;

namespace ticket_management.contract
{
    public interface ITicketService
    {
        Task<Ticket> CreateTicket(string query, string userEmail);
        Task EditTicket(string ticketid, string status, string priority, string intent, int feedbackscore, string agentemailid);
        Task<AnalyticsUIDto> GetAnalytics(string agentEmail);
        Task<Ticket> GetById(string id);
        Task<TicketCount> GetCount(string agentEmailId);
        PagedList<Ticket> GetTickets(string agentEmailId, string userEmailId, string priority, string status, int pageno, int size);
        Task<List<TopAgentsDto>> GetTopAgents();
        Task<Analytics> PushAnalytics();
        Task GetAgents();
        Task<string> AssignEmail(string id);
        IEnumerable<Ticket> getTickets();
    }
}
