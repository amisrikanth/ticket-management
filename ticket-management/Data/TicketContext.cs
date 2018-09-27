using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Microsoft.Extensions.Options;

namespace ticket_management.Models
{
    public class TicketContext
    {

        MongoClient _client;
        IMongoDatabase _db;

        public TicketContext (IOptions<Settings> settings)
        {
            _client = new MongoClient(settings.Value.ConnectionString);
            _db = _client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<Ticket> TicketCollection
        {
            get
            {
                return _db.GetCollection<Ticket>("ticket");
            }
        }

        public IMongoCollection<Analytics> AnalyticsCollection
        {
            get
            {
                return _db.GetCollection<Analytics>("analytics");
            }
        }
        public IMongoCollection<Agents> AgentsCollection
        {
            get
            {
                return _db.GetCollection<Agents>("agents");
            }
        }
        public IMongoCollection<EndUser> EndUsersCollection
        {
            get
            {
                return _db.GetCollection<EndUser>("endUsers");
            }
        }
    }
}
