using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace pos_backoffice.Models.DTOs
{
    public class OrderItemDto
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
