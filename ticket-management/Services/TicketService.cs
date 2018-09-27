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
            GetAgents().Wait();
            GetEndUsers().Wait();

        }

        public IEnumerable<Ticket> getTickets()
        {
            return _context.TicketCollection.AsQueryable().ToList();
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


            var ticket = new TicketCount();

            //Open = await _context.Ticket.Where(x => (x.Status == Status.open&&x.Agentid==agentId&&x.Departmentid== departmentid)).CountAsync(),
            //Closed = await _context.Ticket.Where(x => (x.Status == Status.close && x.Agentid == agentId && x.Departmentid == departmentid)).CountAsync(),
            //Due = await _context.Ticket.Where(x => (x.Status == Status.due && x.Departmentid == departmentid)).CountAsync(),
            //Total = await _context.Ticket.Where(x=> (x.Departmentid == departmentid)).CountAsync()
            ticket.Open = _context.TicketCollection.AsQueryable().Where(x => x.AgentEmailid == agentemailid && x.Status == "open").ToList().Count;
            ticket.Closed = _context.TicketCollection.AsQueryable().Where(x => x.AgentEmailid == agentemailid && x.Status == "closed").ToList().Count;
            ticket.Due = _context.TicketCollection.AsQueryable().Where(x => x.Status == "due").ToList().Count;
            ticket.Total = await _context.TicketCollection.CountDocumentsAsync(new BsonDocument());
                return (ticket);
        }

        public async Task<string> AssignEmail(string id) {
            //_context.TicketCollection.AsQueryable().Where(x => x.TicketId == id).ToList()[0];            
            string agentEmailId = "";
            var filterEmail = Builders<Ticket>.Filter;
            try
            {
                var agentfilter = Builders<Agents>.Filter;
                long ticketCount = await _context.TicketCollection.CountDocumentsAsync(new BsonDocument()) - await _context.TicketCollection.Find(filterEmail.Eq("AgentEmailid", "bot")).CountDocumentsAsync();
                long agentCount = _context.AgentsCollection.CountDocuments(new BsonDocument());
                
                long agentId = (ticketCount + 1) % agentCount;
                agentEmailId = _context.AgentsCollection.Find(agentfilter.Eq("Id", agentId)).ToList()[0].Email;
            }
            catch (Exception e){
                Console.WriteLine(e);
                agentEmailId = "null";
            }
            var filter = Builders<Ticket>.Filter;
            var update = Builders<Ticket>.Update;
            await _context.TicketCollection.UpdateOneAsync(filter.Eq("TicketId", id),update.Set(x => x.AgentEmailid , agentEmailId));
            return agentEmailId;
        }

        //done
        public async Task<Ticket> CreateTicket(string query , string useremail)
        {
            Ticket ticket = new Ticket
            {
                TicketId = ObjectId.GenerateNewId().ToString(),

                AgentEmailid = "bot",
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
                        Count = await _context.TicketCollection.AsQueryable().Where(x => x.Status == "close").CountAsync(),
                        Tickettype = "Closed"
                    },
                    new AnalyticsCountDto
                    {
                        Count = await _context.TicketCollection.AsQueryable().Where(x => x.Status == "due").CountAsync(),
                        Tickettype = "Due"
                    },
                    new AnalyticsCountDto
                    {
                        Count = await _context.TicketCollection.AsQueryable().Where(x => x.Status == "open").CountAsync(),
                        Tickettype = "Open"
                    }
                }
            );
            DateTime date = DateTime.Now;
            Analyticsdata.Avgresolutiontime = _context.AnalyticsCollection.AsQueryable().Where(x => x.Date.Date == date.Date).Select(x => x.Avgresolutiontime).ToList()[0];
            return Analyticsdata;
        }


        //done
        public async Task EditTicket(string ticketid, string status, string priority , string intent, int feedbackscore, string agentemailid)
        {

            var sid = new ObjectId(ticketid);
            var filter = Builders<Ticket>.Filter.Eq("TicketId", sid);
            var ticket =  await _context.TicketCollection.Find(filter).FirstOrDefaultAsync();


            var update = Builders<Ticket>.Update
                        .Set(x => x.Status, status ?? ticket.Status)
                        .Set(x => x.Priority, priority ?? ticket.Priority)
                        .Set(x => x.Intent, intent ?? ticket.Intent)
                        .Set(x => x.Feedbackscore, (feedbackscore == 0) ? 0 : ticket.Feedbackscore)
                        .Set(x => x.Closedon , (status == "close") ? DateTime.Now : ticket.Closedon)
                        .Set(x => x.Closedby, (status == "close") ? agentemailid : ticket.Closedby)
                        .Set(x => x.UpdatedOn, DateTime.Now)
                        .Set(x=> x.UpdatedBy , agentemailid);           
            
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

            return new PagedList<Ticket>(_context.TicketCollection.AsQueryable().Where(x =>
            (string.IsNullOrEmpty(status) || x.Status == status) &&
            (string.IsNullOrEmpty(priority) || x.Priority == priority) &&
            (string.IsNullOrEmpty(useremailid) || x.UserEmailId == useremailid) &&
            (string.IsNullOrEmpty(agentemailid) || x.AgentEmailid == agentemailid)
            ).ToList(), pageno, size);

            
        }
        
        
        //Update analytics csat score //done
        public async Task<Analytics> PushAnalytics()
        {
            DateTime date = DateTime.Now;
            List<int?> ticketscore = new List<int?>();

            ticketscore = _context.TicketCollection.AsQueryable()
                .Where(x => 
                x.UpdatedOn.Value.ToString().Split()[0] == date.Date.ToString().Split()[0] &&
                x.Status == "close" &&
                x.Feedbackscore > 3)
                .Select(x => x.Feedbackscore).ToList();

            List<int?> totalticketcount = new List<int?>();

            totalticketcount = _context.TicketCollection.AsQueryable()
                .Where(x =>
                x.UpdatedOn.Value.ToString().Split()[0] == date.Date.ToString() &&
                x.Status == "close" &&
                x.Feedbackscore > 0)
                .Select(x => x.Feedbackscore).ToList();
            double csatscore;
            try
            {
                csatscore = (double)ticketscore.Sum() / totalticketcount.Count();
            }
            catch {
                csatscore = 0;
            }
            HttpClient http = new HttpClient();
            string url = "http://localhost:3000/getIntent";
            var response = await http.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            List<IntentDto> intents = JsonConvert.DeserializeObject<List<IntentDto>>(result);
            List<Ticket> listOfTickets = new List<Ticket>();
            List<AvgResolutionTime> avgResolutionTime = new  List<AvgResolutionTime>();

            foreach (IntentDto intent in intents) {
                TimeSpan totalhours = new TimeSpan();
                AvgResolutionTime avgresolutiondata = new AvgResolutionTime();
                listOfTickets = _context.TicketCollection.AsQueryable().Where(x => x.Status == "close" && x.Intent == intent.name).ToList();                
                avgresolutiondata.intent = intent.name;                
                foreach (Ticket ticket in listOfTickets)
                {
                    totalhours += (DateTime)ticket.Closedon - (DateTime)ticket.CreatedOn;
                }
                avgresolutiondata.avgresolutiontime = totalhours.Hours;
                avgResolutionTime.Add(avgresolutiondata);
            }
            csatscore = (double)ticketscore.Sum() / totalticketcount.Count();
            Analytics scheduledData = new Analytics
            {
                Date = date.Date,
                Avgresolutiontime = avgResolutionTime,
                Csatscore = csatscore
            };
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

        public async Task GetAgents()
        {
            HttpClient httpclient = new HttpClient();
            string url = "http://35.221.76.107/agents";
            var response = await httpclient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            Agents[] responsejson = JsonConvert
                    .DeserializeObject<Agents[]>(result);
            await _context.AgentsCollection.InsertManyAsync(responsejson);

        }
        public async Task GetEndUsers()
        {
            HttpClient httpclient = new HttpClient();
            string url = "http://35.221.76.107/endusers";
            var response = await httpclient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            EndUser[] responsejson = JsonConvert
                    .DeserializeObject<EndUser[]>(result);
            await _context.EndUsersCollection.InsertManyAsync(responsejson);
        }

    }

}

