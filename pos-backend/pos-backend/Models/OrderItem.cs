using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace pos_backend.Models
{
    public class OrderItem
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public required string ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }  
        public int Quantity { get; set; }
    }
}
