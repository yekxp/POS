using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace pos_backoffice.Models.DTOs
{
    public class OrderDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public DateTime CreatedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int Status { get; set; }
        public int PaymentType { get; set; }
        public string TableNumber { get; set; }
    }
}
