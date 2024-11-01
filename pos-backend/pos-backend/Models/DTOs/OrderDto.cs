using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace pos_backend.Models.DTOs
{
    public class OrderDto
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public List<OrderItemDto>? Items { get; set; } = [];

        public DateTime? CreatedDate { get; set; }

        public decimal? TotalAmount { get; set; }

        [BsonRepresentation(BsonType.String)]
        public PaymentStatus? Status { get; set; }

        public string TableNumber { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.String)]
        public Location Location { get; set; }
    }
}
