using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace pos_backoffice_user_managment.Database
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
