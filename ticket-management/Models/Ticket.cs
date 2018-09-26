using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ticket_management.Models
{
    
    public class Ticket
    {
        [BsonElement("TicketId")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TicketId { get; set; }
        [BsonElement("Intent")]
        public string Intent { get ; set ; }
        [BsonElement("Description")]
        public string Description { get ; set ; }
        [BsonElement("AgentEmailid")]
        public string AgentEmailid { get ; set ; }
        [BsonElement("Priority")]
        public string Priority { get; set; }
        [BsonElement("Status")]
        public string Status { get ; set ; }
        [BsonElement("CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [BsonElement("UpdatedOn")]
        public DateTime? UpdatedOn { get; set; }
        [BsonElement("UserEmailId")]
        public string UserEmailId { get; set ; }
        [BsonElement("UpdatedBy")]
        public string UpdatedBy { get ; set ; }
        [BsonElement("Closedon")]
        public DateTime? Closedon { get; set ; }
        [BsonElement("Closedby")]
        public string Closedby { get ; set ; }
        [BsonElement("Feedbackscore")]
        public int? Feedbackscore { get; set; }
    }

}
