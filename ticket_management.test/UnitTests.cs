//using System;
//using Xunit;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Hosting;
//using ticket_management;
//using System.Net.Http;
//using Microsoft.AspNetCore.TestHost;
//using Microsoft.AspNetCore.Mvc;
//using ticket_management.Models;
//using System.Collections.Generic;
//using System.Text;
//using Newtonsoft.Json;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using FluentAssertions;
//using System.Net;

//namespace ticket_management.test
//{
//    public class TicketTest
//    {
//        public HttpClient _client;

//        Ticket t1 = new Ticket()
//        {
//            Agentid = 1,
//            Comment = new List<Comments>
//        {
//                new Comments{
//            Comment = "Hello World",
//            CreatedBy = 1,
//            CreatedOn = DateTime.Now,
//            UpdatedBy = 1,
//            UpdatedOn = DateTime.Now
//            }
//        },
//            Customerid = 1,
//            Userid = 1,
//            Subject = "unassigned",
//            Status = Status.open,
//            Departmentid = 1,
//            Description = "Hello Wolrd",
//            Priority = "High",
//            Sla = 1321,
//            Source = "Twitter",
//            CreatedBy = 1,
//            CreatedOn = DateTime.Now,
//            UpdatedBy = 1,
//            UpdatedOn = DateTime.Now
//        };
//        Ticket t2 = new Ticket()
//        {
//            Agentid = 1,
//            Comment = new List<Comments>
//        {
//                new Comments{
//            Comment = "Heey",
//            CreatedBy = 1,
//            CreatedOn = DateTime.Now,
//            UpdatedBy = 1,
//            UpdatedOn = DateTime.Now
//            }
//        },
//            Customerid = 1,
//            Userid = 1,
//            Subject = "unassigned",
//            Status = Status.due,
//            Departmentid = 1,
//            Description = "Hello",
//            Priority = "Low",
//            Sla = 324,
//            Source = "Chat",
//            CreatedBy = 1,
//            CreatedOn = DateTime.Now,
//            UpdatedBy = 1,
//            UpdatedOn = DateTime.Now
//        };

//        Ticket editticket = new Ticket()
//        {
//            TicketId = 2,
//            Agentid = 1,
//            Comment = new List<Comments>
//        {
//          new Comments{
//            CommentId = 2,
//            Comment = "Heey",
//            CreatedBy = 2,
//            CreatedOn = DateTime.Now,
//            UpdatedBy = 2,
//            UpdatedOn = DateTime.Now
//            }
//        },
//            Customerid = 1,
//            Userid = 1,
//            Subject = "Management",
//            Status = Status.due,
//            Departmentid = 1,
//            Description = "Hello World",
//            Priority = "Low",
//            Sla = 324,
//            Source = "Chat",
//            CreatedBy = 1,
//            CreatedOn = DateTime.Now,
//            UpdatedBy = 1,
//            UpdatedOn = DateTime.Now
//        };

//        ChatDto createticket = new ChatDto()
//        {
//            Description = "Heey",
//            Customerhandle = "something@gmail.com",
//            Connectionid = "Twitter/124",
//            Userid = 1
//        };

//        List<Ticket> dbticket = new List<Ticket>();

//        public TicketContext _context;

//        public TicketTest()
//        {
//            var builder = new WebHostBuilder()
//                .UseEnvironment("Testing")
//                .UseStartup<Startup>();

//            TestServer testServer = new TestServer(builder);

//            _client = testServer.CreateClient();
//            _context = testServer.Host.Services.GetRequiredService<TicketContext>();
//            dbticket.Add(t1);
//            dbticket.Add(t2);
//            _context.Ticket.Add(t1);
//            _context.Ticket.Add(t1);
//            _context.SaveChanges();
//        }

//        [Fact]
//        public async void GetAllTicket()
//        {
//            var response = await _client.GetAsync("/api/Tickets");

//            response.EnsureSuccessStatusCode();
//            var result = await response.Content.ReadAsStringAsync();


//            Assert.NotNull(result);

//            response.StatusCode.Should().Be(HttpStatusCode.OK);
//            var responseString = await response.Content.ReadAsStringAsync();

//            var tickets = JsonConvert.DeserializeObject<List<Ticket>>(responseString);

//            responseString.Should().Contain("Hello World")
//              .And.Contain("Twitter")
//              .And.Contain("High");

//            //Assert.True(tickets.TrueForAll(x => dbticket.Exists(y => y.IsEquals(x))));

//        }

//        [Fact]
//        public async void GetTicketBYId()
//        {
//            var response = await _client.GetAsync("/api/Tickets/detail/1");

//            response.EnsureSuccessStatusCode();
//            var result = await response.Content.ReadAsStringAsync();


//            Assert.NotNull(result);

//            response.StatusCode.Should().Be(HttpStatusCode.OK);
//            var responseString = await response.Content.ReadAsStringAsync();

//            //var tickets = JsonConvert.DeserializeObject<List<Ticket>>(responseString);

//            responseString.Should().Contain("Hello World")
//              .And.Contain("Twitter")
//              .And.Contain("High");

//            //Assert.True(tickets.TrueForAll(x => dbticket.Exists(y => y.IsEquals(x))));

//        }

//        [Fact]
//        public async void GetCount()
//        {
//            var response = await _client.GetAsync("/api/Tickets/count");

//            response.EnsureSuccessStatusCode();
//            var result = await response.Content.ReadAsStringAsync();


//            Assert.NotNull(result);

//            response.StatusCode.Should().Be(HttpStatusCode.OK);
//            var responseString = await response.Content.ReadAsStringAsync();

//        }

//        [Fact]
//        public async void GetTicketByStatus()
//        {
//            var response = await _client.GetAsync("/api/Tickets/status/open");

//            response.EnsureSuccessStatusCode();
//            var result = await response.Content.ReadAsStringAsync();


//            Assert.NotNull(result);

//            response.StatusCode.Should().Be(HttpStatusCode.OK);
//            var responseString = await response.Content.ReadAsStringAsync();

//        }

//        [Fact]
//        public async void CreateTicket()
//        {
//            HttpRequestMessage postMessage = new HttpRequestMessage(HttpMethod.Post, "api/Tickets")
//            {
//                Content = new StringContent(JsonConvert.SerializeObject(createticket), UnicodeEncoding.UTF8, "application/json")
//            };
//            var response = await _client.SendAsync(postMessage);

//            var responseString = await response.Content.ReadAsStringAsync();
            

//            response.EnsureSuccessStatusCode();

            

//        }

//        [Fact]
//        public async void EditTicket()
//        {
//            HttpRequestMessage putMessage = new HttpRequestMessage(HttpMethod.Put, "api/Tickets/2")
//            {
//                Content = new StringContent(JsonConvert.SerializeObject(editticket), Encoding.UTF8, "application/json")
//            };

//            var response = await _client.SendAsync(putMessage);

//            var responseString = await response.Content.ReadAsStringAsync();
//            //var obj = JsonConvert.DeserializeObject<Note>(responseString);

//            var statuscode = response.StatusCode;
//            Assert.Equal(HttpStatusCode.Created, statuscode);

//            //Assert.True(putnote2.IsEquals(obj));
//        }

//    }
//}
