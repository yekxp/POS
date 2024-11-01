using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace pos_backend.Models.DTOs
{
    public record CashRegisterOpenDto
    {
        public decimal InitialCash { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Location Location { get; set; }
    }

    public record CashRegisterCloseDto
    {
        [BsonRepresentation(BsonType.String)]
        public Location Location { get; set; }
    }

    public record CashInDto
    {
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
    }

    public record CashOutDto
    {
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
    }

    public record CashRegisterDto
    {
        public string? Id { get; set; }

        public decimal InitialCash { get; set; }

        public decimal TotalCash { get; set; }

        public bool IsOpen { get; set; }

        public DateTime OpenedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Location Location { get; set; }
    }
}
