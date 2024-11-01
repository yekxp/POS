namespace pos_backend.Models.DTOs
{
    public record ReceiptDto
    {
        public string? Id { get; set; }
        public string? PaymentId { get; set; }
        public DateTime Date { get; set; }
        public List<OrderItemDto> Items { get; set; } = [];
        public decimal TotalAmount { get; set; }
        public decimal CashPaid { get; set; }
        public decimal ChangeGiven { get; set; }
    }
}
