using pos_backoffice.Models.DTOs;

namespace pos_backoffice.Services
{
    public interface IReportService
    {
        Task<SalesReportDto> GetSalesReportAsync(DateTime startDate, DateTime endDate);
    }
}
