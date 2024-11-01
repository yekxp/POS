using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace pos_backend.Models
{
    public class Receipt
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PaymentId { get; set; }
        public DateTime Date { get; set; }
        public List<OrderItem> Items { get; set; } = [];
        public decimal TotalAmount { get; set; }
        public decimal CashPaid { get; set; }
        public decimal ChangeGiven { get; set; }
    }
}
