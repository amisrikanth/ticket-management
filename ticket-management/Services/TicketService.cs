using Microsoft.EntityFrameworkCore;
using ticket_management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ticket_management.contract;
using RabbitMQ.Client;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ticket_management.Services
{
    public class TicketService : ITicketService
    {
        private readonly TicketContext _context;

        public TicketService(IOptions<Settings> settings)
        {
            _context = new TicketContext(settings);  
        }

        //done
        public async Task<Ticket> GetById(string id)
        {
            //Ticket CompleteTicketDetails = await _context.Ticket
            //                                    .Include(x => x.Comment)
            //                                    .SingleOrDefaultAsync(x => x.TicketId == id);


            //TicketDetailsDto Ticket = new TicketDetailsDto
            //{
            //    Id = CompleteTicketDetails.TicketId,
            //    Name = "userName",
            //    Priority = CompleteTicketDetails.Priority,
            //    Status = CompleteTicketDetails.Status,
            //    Subject = CompleteTicketDetails.Intent,
            //    Description = CompleteTicketDetails.Description,
            //};
            //return Ticket;

            var filter = Builders<Ticket>.Filter.Eq("TicketId", id);
            var ticket = await _context.TicketCollection.Find(filter).FirstOrDefaultAsync();
            return ticket;

        }

        //done
        public async Task<TicketCount> GetCount(string agentemailid)
        {
            //string AgentEmailid

            var filter = Builders<Ticket>.Filter;

            return new TicketCount
            {
                //Open = await _context.Ticket.Where(x => (x.Status == Status.open&&x.Agentid==agentId&&x.Departmentid== departmentid)).CountAsync(),
                //Closed = await _context.Ticket.Where(x => (x.Status == Status.close && x.Agentid == agentId && x.Departmentid == departmentid)).CountAsync(),
                //Due = await _context.Ticket.Where(x => (x.Status == Status.due && x.Departmentid == departmentid)).CountAsync(),
                //Total = await _context.Ticket.Where(x=> (x.Departmentid == departmentid)).CountAsync()
                Open = await _context.TicketCollection.Find(filter.Eq("AgentEmailid", agentemailid) & filter.Eq("Status", "open")).CountDocumentsAsync(),
                Closed = await _context.TicketCollection.Find(filter.Eq("AgentEmailid", agentemailid) & filter.Eq("Status", "close")).CountDocumentsAsync(),
                Due = await _context.TicketCollection.Find(filter.Eq("AgentEmailid", agentemailid) & filter.Eq("Status", "due")).CountDocumentsAsync(),
                Total = await _context.TicketCollection.Find(filter.Eq("AgentEmailid", agentemailid)).CountDocumentsAsync(),

            };
        }

        //done
        public async Task<Ticket> CreateTicket(string query , string useremail)
        {
            Ticket ticket = new Ticket
            {
                TicketId = ObjectId.GenerateNewId().ToString(),
                AgentEmailid = null,
                Closedby = null,
                Closedon = null,
                CreatedOn = DateTime.Now,
                Description = query,
                Intent = null,
                Feedbackscore = null,
                Priority = "Low",
                Status = "open",
                UpdatedBy = null,
                UpdatedOn = null,
                UserEmailId = useremail
            };

            //_context.Ticket.Add(ticket);
            //await _context.SaveChangesAsync();
            //return ticket;

             await _context.TicketCollection.InsertOneAsync(ticket);
             return (ticket);

        }

        //done
        public async Task<AnalyticsUIDto> GetAnalytics(string agentemail)
        {
            AnalyticsUIDto Analyticsdata = new AnalyticsUIDto
            {
                Analyticscsat = new List<AnalyticsCsatDto>()
            };

            Analyticsdata.Analyticscsat.AddRange(
                _context.AnalyticsCollection.AsQueryable().Select(
                    x => new AnalyticsCsatDto { Date = x.Date, Csatscore = x.Csatscore }
                    )
                );

            TicketCount count = new TicketCount();
            Analyticsdata.Analyticscount = new List<AnalyticsCountDto>();
            Analyticsdata.Analyticscount.AddRange(
                new List<AnalyticsCountDto> {
                    new AnalyticsCountDto
                    {
                        Count = await _context.TicketCollection.AsQueryable().Where(x => (x.Status == "close" && x.AgentEmailid == agentemail)).CountAsync(),
                        Tickettype = "Closed"
                    },
                    new AnalyticsCountDto
                    {
                        Count = await _context.TicketCollection.AsQueryable().Where(x => (x.Status == "due" && x.AgentEmailid == agentemail)).CountAsync(),
                        Tickettype = "Due"
                    },
                    new AnalyticsCountDto
                    {
                        Count = await _context.TicketCollection.AsQueryable().Where(x => (x.Status == "open" && x.AgentEmailid == agentemail)).CountAsync(),
                        Tickettype = "Open"
                    }
                }
            );
            Analyticsdata.Avgresolutiontime = "5:04:23";
            return Analyticsdata;
        }


        //done
        public async Task EditTicket(string ticketid,string status, string priority , string intent)
        {

            var sid = new ObjectId(ticketid);
            var filter = Builders<Ticket>.Filter.Eq("TicketId", sid);
            var ticket =  await _context.TicketCollection.Find(filter).FirstOrDefaultAsync();


            var update = Builders<Ticket>.Update
                        .Set(x => x.Status, status ?? ticket.Status)
                        .Set(x => x.Priority, priority ?? ticket.Priority)
                        .Set(x => x.Intent, intent ?? ticket.Intent);
            
            _context.TicketCollection.UpdateOne(filter, update);
        }


        //done
        public PagedList<Ticket> GetTickets(string agentemailid, string useremailid , string priority, string status, int pageno, int size)
        {
            if (pageno == 0 || size == 0)
            {
                pageno = 1;
                size = 20;
            }
            // return _context.TicketCollection.Where(n =>
            //(
            //    n.Agentid == ((agentid != 0) ? agentid : n.Agentid) &&
            //    n.Departmentid == ((departmentid != 0) ? departmentid : n.Departmentid) &&
            //    n.Userid == ((userid != 0) ? userid : n.Userid) &&
            //    n.Customerid == ((customerid != 0) ? customerid : n.Customerid) &&
            //    n.Source == (String.IsNullOrEmpty(source) ? n.Source : source) &&
            //    n.Priority == (String.IsNullOrEmpty(priority) ? n.Priority : priority) &&
            //    n.Status.ToString() == (String.IsNullOrEmpty(status) ? n.Status.ToString() : status)
            //)
            //).Skip((pageno - 1) * size).Take(size);

            return new PagedList<Ticket>(_context.TicketCollection.AsQueryable().Where(x =>
            (string.IsNullOrEmpty(status) || x.Status == status) &&
            (string.IsNullOrEmpty(priority) || x.Priority == priority) &&
            (string.IsNullOrEmpty(useremailid) || x.UserEmailId == useremailid) &&
            (string.IsNullOrEmpty(agentemailid) || x.AgentEmailid == agentemailid)
            ).ToList(), pageno, size);
            //.Skip((pageno - 1) * size).Take(size).ToList();
            
        }

        
        //Update analytics csat score //done
        public async Task<Analytics> PushAnalytics()
        {
            DateTime date = DateTime.Now;
            List<int?> ticketscore = new List<int?>();


            ticketscore = _context.TicketCollection.AsQueryable()
                .Where(x => 
                x.UpdatedOn.Value.ToString().Split()[0] == date.Date.ToString() &&
                x.Status == "close" &&
                x.Feedbackscore > 3)
                .Select(x => x.Feedbackscore).ToList();



            List<int> totalticketscore = new List<int>();

            ticketscore = _context.TicketCollection.AsQueryable()
                .Where(x =>
                x.UpdatedOn.Value.ToString().Split()[0] == date.Date.ToString() &&
                x.Status == "close" &&
                x.Feedbackscore > 0)
                .Select(x => x.Feedbackscore).ToList();

            Console.WriteLine((double)ticketscore.Sum() / totalticketscore.Count());
            Console.WriteLine(ticketscore.Sum());
            Console.WriteLine(totalticketscore.Count());
            double csatscore = (double)ticketscore.Sum() / totalticketscore.Count();
            Analytics scheduledData = new Analytics();
            scheduledData.Date = date.Date;
            scheduledData.Customerid = '1';
            scheduledData.Avgresolutiontime = "5:0:0";
            scheduledData.Csatscore = csatscore;
            await _context.AnalyticsCollection.InsertOneAsync(scheduledData);
            return scheduledData;
        }
        
        //done
        public async Task<List<TopAgentsDto>> GetTopAgents()
        {
            HttpClient httpclient = new HttpClient();
            var listOfAgents = _context.TicketCollection.AsQueryable().Where(x => x.Status == "close")
                .GroupBy(x => x.AgentEmailid).OrderByDescending(x => x.Count()).Take(3).ToList();
            List<TopAgentsDto> agentsList = new List<TopAgentsDto>();
            
            foreach(var agentTickets in listOfAgents)
            {
                
                string url = "http://35.221.125.153:8082/api/agents/leaderboard?id="
                    + agentTickets.Key;
                var response = await httpclient.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                TopAgentsDto responsejson = JsonConvert
                    .DeserializeObject<TopAgentsDto>(result);
                TopAgentsDto agent = new TopAgentsDto
                {
                    NumberOfTicketsResolved = agentTickets.Count(),
                    Name = responsejson.Name,
                    DepartmentName = responsejson.DepartmentName,
                    ProfileImageUrl = responsejson.ProfileImageUrl
                };
                agentsList.Add(agent);
            }

            return agentsList;
        }
    }

}

