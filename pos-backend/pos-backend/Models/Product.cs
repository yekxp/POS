using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace pos_backend.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        public required string Name { get; set; }

        public decimal Price { get; set; }

        public string? Description { get; set; }

        [BsonRepresentation(BsonType.String)]
        public List<Location>? Location { get; set; }
    }

}
