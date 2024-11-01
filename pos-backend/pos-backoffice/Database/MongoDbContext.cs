using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace pos_backoffice.Database
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDBSettings> settings, IMongoClient mongoClient)
        {
            _database = mongoClient.GetDatabase("POSDatabase");
        }
    }
}
