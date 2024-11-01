using Microsoft.Extensions.Options;
using MongoDB.Driver;
using pos_backend.Models;

namespace pos_backend.Database
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDBSettings> settings, IMongoClient mongoClient)
        {
            _database = mongoClient.GetDatabase("POSDatabase");
        }

        public IMongoCollection<Order> Orders => _database.GetCollection<Order>(nameof(Orders));
    }
}
