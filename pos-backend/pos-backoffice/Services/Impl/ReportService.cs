using Microsoft.Extensions.Options;
using MongoDB.Driver;
using pos_backoffice.Database;
using pos_backoffice.Models.DTOs;

namespace pos_backoffice.Services.Impl
{
    public class ReportService : IReportService
    {
        private readonly IMongoCollection<OrderDto> _ordersCollection;

        public ReportService(IOptions<MongoDBSettings> settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _ordersCollection = database.GetCollection<OrderDto>(settings.Value.OrdersCollectionName);
        }

        public async Task<SalesReportDto> GetSalesReportAsync(DateTime startDate, DateTime endDate)
         {
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

            var filter = Builders<OrderDto>.Filter.Gte(o => o.CreatedDate, startDate) &
                         Builders<OrderDto>.Filter.Lte(o => o.CreatedDate, endDate);

            List<OrderDto> orders = await _ordersCollection.Find(filter).ToListAsync();

            decimal totalSales = orders.Sum(o => o.TotalAmount);
            int totalTransactions = orders.Count;

            return new SalesReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                ReportDate = DateTime.UtcNow,
                TotalSales = totalSales,
                TotalTransactions = totalTransactions
            };
        }
    }
}
