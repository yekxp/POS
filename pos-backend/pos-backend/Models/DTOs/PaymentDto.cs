using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace pos_backend.Models.DTOs
{
    public class PaymentDto
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string OrderId { get; set; }

        public decimal Amount { get; set; }

        [BsonRepresentation(BsonType.String)]
        public PaymentType PaymentType { get; set; }

        public DateTime PaymentDate { get; set; }

        [BsonRepresentation(BsonType.String)]
        public PaymentStatus? Status { get; set; }
        
        public decimal RefundAmount { get; set; }
        
        public string? RefundReason { get; set; }
        
        public DateTime? RefundDate { get; set; }
    }

}
