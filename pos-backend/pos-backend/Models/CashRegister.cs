using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace pos_backend.Models
{
    public class CashRegister
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public decimal InitialCash { get; set; }

        public decimal TotalCash { get; set; }
        
        public bool IsOpen { get; set; }
        
        public DateTime OpenedAt { get; set; }
        
        public DateTime? ClosedAt { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Location Location { get; set; }
    }
}
