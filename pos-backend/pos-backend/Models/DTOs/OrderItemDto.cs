namespace pos_backend.Models.DTOs
{
    public class OrderItemDto
    {
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
    }

}
