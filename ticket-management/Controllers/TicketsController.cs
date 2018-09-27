using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticket_management.Models;
using ticket_management.contract;
using static ticket_management.Models.OnboardingUser;
using Newtonsoft.Json;

namespace ticket_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        //private readonly IUrlHelper _urlHelper;

        public TicketsController(ITicketService ticketService)
        {
            //_urlHelper = urlHelper;
            _ticketService = ticketService;
        }
      
        [Route("leaderboard")]
        public async Task<IActionResult> GetTopAgents()
        {
            var topAgents = await _ticketService.GetTopAgents();
            return Ok(topAgents);
        }



        [Route("detail/{id}")]
        public async Task<IActionResult> GetTicketById([FromRoute] string id)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ticket ticket = await _ticketService.GetById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }


        //for user
        [Route("count")]
        public async Task<TicketCount> CountTickets([FromQuery] string agentEmailId)
        {
            return await _ticketService.GetCount(agentEmailId);
    
        }

        

        [Route("filter")]
        public IActionResult GetSortedTickets([FromHeader] string agentEmailId,
            [FromQuery] string userEmailId,
            [FromQuery] string priority,
            [FromQuery] string status,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize)
        {
            var model = _ticketService.GetTickets(agentEmailId, userEmailId, priority, status, pageNumber, pageSize);
            TicketOutputModel outputModel = new TicketOutputModel
            {
                Pages = model.TotalPages,
                HasNext = model.HasNextPage,
                HasPrevious = model.HasPreviousPage,
                Tickets = model.List
            };

            return Ok(outputModel);
        }

        [HttpGet("Analytics")]
        public async Task<AnalyticsUIDto> GetAnalytics([FromHeader] string agentEmail)
        {
            return await _ticketService.GetAnalytics(agentEmail);
        }

        [HttpGet()]
        public IEnumerable<Ticket> Gettickets()
        {
            return  _ticketService.getTickets();
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateTicket([FromQuery] string query, [FromQuery] string userEmail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ticket ticket = await _ticketService.CreateTicket( query, userEmail); ;
            return Ok(ticket);
        }

        [HttpGet("Assignagent/{id}")]
        public async Task<IActionResult> assignAgent([FromRoute] string id) {
            string Email = await _ticketService.AssignEmail(id);
            return Ok(Email);
        } 
        [HttpGet("analytics/update")]
        public async Task<IActionResult> CreateAnalysis()
        {
            Analytics analyticsdata = await _ticketService.PushAnalytics();
            return Ok(analyticsdata);

        }

        // PUT: api/Tickets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditTicket([FromRoute]string id, [FromQuery] string status, [FromQuery] string priority, [FromQuery] string intent, [FromQuery] int feedbackscore, [FromQuery] string agentemailid)
        {
            await _ticketService.EditTicket(id, status, priority, intent, feedbackscore, agentemailid);
            return Ok();
        }
       
    }
}