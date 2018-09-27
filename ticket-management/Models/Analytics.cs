using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ticket_management.Models
{
    public class Analytics
    {
        [BsonElement("Id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get ; set; }
        [BsonElement("Date")]
        public DateTime Date { get; set ; }

        [BsonElement("AvgResolutionTime")]
        public List<AvgResolutionTime> Avgresolutiontime { get ; set ; } 
        [BsonElement("Csatscore")]
        public double Csatscore { get ; set ; }
        [BsonElement("Customerid")]
        public int Customerid { get ; set ; }
    }
}
