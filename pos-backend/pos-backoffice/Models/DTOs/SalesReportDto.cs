namespace pos_backoffice.Models.DTOs
{
    public class SalesReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalTransactions { get; set; }
    }
}
