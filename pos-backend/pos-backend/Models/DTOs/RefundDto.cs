namespace pos_backend.Models.DTOs
{
    public class RefundDto
    {
        public decimal RefundAmount { get; set; }
        public string? Reason { get; set; }
        public DateTime RefundDate { get; set; } = DateTime.UtcNow;
    }

}
