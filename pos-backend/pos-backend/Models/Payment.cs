using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace pos_backend.Models
{
    public class Payment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public required string OrderId { get; set; }

        public decimal Amount { get; set; }

        [BsonRepresentation(BsonType.String)]
        public required PaymentType PaymentType { get; set; }

        public DateTime PaymentDate { get; set; }

        [BsonRepresentation(BsonType.String)]
        public PaymentStatus Status { get; set; }

        public decimal? RefundAmount { get; set; }
        
        public string? RefundReason { get; set; }
        
        public DateTime? RefundDate { get; set; }
    }
}
