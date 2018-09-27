using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ticket_management.Models
{
    public class Organisation
    {
        [BsonElement("OrganisationId")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string OrganisationId { get; set; }
        [BsonElement("Id")]
        public long Id { get; set; }
        [BsonElement("Email")]
        public string Email { get; set; }
        [BsonElement("Password")]
        public string Password { get; set; }
        [BsonElement("CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [BsonElement("CreatedBy")]
        public long CreatedBy { get; set; }
        [BsonElement("UpdatedOn")]
        public DateTime UpdatedOn { get; set; }
        [BsonElement("UpdatedBy")]
        public long UpdatedBy { get; set; }
        [BsonElement("OrganisationName")]
        public string OrganisationName { get; set; }
        [BsonElement("OrganisationDisplayName")]
        public string OrganisationDisplayName { get; set; }
    }
}
