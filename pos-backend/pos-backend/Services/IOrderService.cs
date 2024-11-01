using pos_backend.Models;
using pos_backend.Models.DTOs;

namespace pos_backend.Services
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllOrdersAsync();

        Task<OrderDto> GetOrderByIdAsync(string id);

        Task<OrderDto> CreateOrderAsync(OrderDto orderDto);

        Task<OrderDto> UpdateOrderAsync(string id, OrderDto orderDto);

        Task<bool> DeleteOrderAsync(string id);

        Task<IEnumerable<OrderDto>> GetOrdersByLocations(Location[] locations);
    }

}
