using AutoMapper;
using pos_backend.Models.DTOs;
using pos_backend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using pos_backend.Database;

namespace pos_backend.Services.Impl
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMapper _mapper;

        public OrderService(IOptions<MongoDBSettings> settings, IMongoClient mongoClient, IMapper mapper)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _ordersCollection = database.GetCollection<Order>(settings.Value.OrdersCollectionName);
            _productCollection = database.GetCollection<Product>(settings.Value.ProductsCollectionName);
            _mapper = mapper;
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            List<Order> orders = await _ordersCollection.Find(order => true).ToListAsync();
            return _mapper.Map<List<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderByIdAsync(string id)
        {
            Order order = await _ordersCollection.Find(o => o.Id == id).FirstOrDefaultAsync();
            if (order == null) 
                return null;
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto)
        {
            foreach (OrderItemDto item in orderDto.Items)
            {
                Product product = await _productCollection.Find(p => p.Id == item.ProductId).FirstOrDefaultAsync();

                if (product == null)
                    return null;

                item.ProductName = product.Name;
                item.Price = product.Price;
            }

            Order order = _mapper.Map<Order>(orderDto);
            order.TotalAmount = order.Items.Sum(item => item.Price * item.Quantity);
            order.CreatedDate = DateTime.UtcNow;

            await _ordersCollection.InsertOneAsync(order);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> UpdateOrderAsync(string id, OrderDto orderDto)
        {
            Order? existingOrder = await _ordersCollection.Find(o => o.Id == id).FirstOrDefaultAsync();
            if (existingOrder == null)
                return null;


            if (orderDto.Items != null && orderDto.Items.Count > 0)
            {
                decimal newTotalAmount = 0;

                foreach (OrderItemDto itemDto in orderDto.Items)
                {
                    Product product = await _productCollection.Find(p => p.Id == itemDto.ProductId).FirstOrDefaultAsync();
                    if (product == null)
                        throw new Exception($"Product with ID {itemDto.ProductId} does not exist.");

                    OrderItem? existingItem = existingOrder.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);

                    if (existingItem != null)
                    {
                        if (itemDto.Quantity == 0)
                        {
                            existingOrder.Items.Remove(existingItem);
                        }
                        else
                        {
                            existingItem.Quantity = itemDto.Quantity;
                            existingItem.Price = product.Price;
                            existingItem.ProductName = product.Name;
                        }
                    }
                    else
                    {
                        if (itemDto.Quantity > 0)
                        {
                            OrderItem newOrderItem = new OrderItem
                            {
                                ProductId = itemDto.ProductId,
                                ProductName = product.Name,
                                Quantity = itemDto.Quantity,
                                Price = product.Price
                            };
                            existingOrder.Items.Add(newOrderItem);
                        }
                    }
                }

                newTotalAmount = existingOrder.Items.Sum(item => item.Price * item.Quantity);
                existingOrder.TotalAmount = newTotalAmount;
            }

            if (!string.IsNullOrEmpty(orderDto.TableNumber)) 
                existingOrder.TableNumber = orderDto.TableNumber;

            if (orderDto.Status is not null || orderDto.Status != default) 
                existingOrder.Status = (PaymentStatus)orderDto.Status!;

            await _ordersCollection.ReplaceOneAsync(o => o.Id == id, existingOrder);

            return _mapper.Map<OrderDto>(existingOrder);
        }

        public async Task<bool> DeleteOrderAsync(string id)
        {
            DeleteResult result = await _ordersCollection.DeleteOneAsync(o => o.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByLocations(Location[] locations)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.In(p => p.Location, locations);

            List<Order> existingOrders = await _ordersCollection.Find(filter).ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(existingOrders);
        }

    }
}
