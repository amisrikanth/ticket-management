﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ticket_management.Models
{
    public class Agents
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AgentId { get; set; }
        [BsonElement("Id")]
        public long Id { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Email")]
        public string Email { get; set; }
        [BsonElement("Organization")]
        public Organisation Organization { get; set; }
        [BsonElement("CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [BsonElement("CreatedBy")]
        public long CreatedBy { get; set; }
        [BsonElement("UpdatedOn")]
        public DateTime UpdatedOn { get; set; }
        [BsonElement("UpdatedBy")]
        public long UpdatedBy { get; set; }
        [BsonElement("PhoneNumber")]
        public string PhoneNumber { get; set; }
        [BsonElement("ProfileImgUrl")]
        public string ProfileImgUrl { get; set; }
    }
}