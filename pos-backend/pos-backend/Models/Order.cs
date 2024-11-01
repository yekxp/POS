using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace pos_backend.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        public List<OrderItem> Items { get; set; } = [];

        public DateTime CreatedDate { get; set; }

        public decimal TotalAmount { get; set; }

        [BsonRepresentation(BsonType.String)]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public string TableNumber { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.String)]
        public Location Location { get; set; }
    }
}
