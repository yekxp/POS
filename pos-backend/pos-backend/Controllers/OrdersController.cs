using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pos_backend.Models;
using pos_backend.Models.DTOs;
using pos_backend.Services;

namespace pos_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllOrders()
        {
            List<OrderDto> orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("order")]
        [Authorize(Roles = "Manager,CashierBA,CashierKE")]
        public async Task<IActionResult> GetOrderById([FromQuery] string id)
        {
            OrderDto order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(new { Message = $"Order with ID {id} not found." });
   
            return Ok(order);
        }

        [HttpGet("locations")]
        [Authorize(Roles = "Manager,CashierBA,CashierKE")]
        public async Task<IActionResult> GetOrdersByLocation([FromQuery] Location[] location)
        {
            IEnumerable<OrderDto> orders = await _orderService.GetOrdersByLocations(location);
            if (orders is null || orders.Count() == 0)
                return NotFound();

            return Ok(orders);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Manager,CashierBA,CashierKE")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            OrderDto createdOrder = await _orderService.CreateOrderAsync(orderDto);
            if (createdOrder is null)
                return NotFound(new { Message = $"Cannot find products." });

            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("update")]
        [Authorize(Roles = "Manager,CashierBA,CashierKE")]
        public async Task<IActionResult> UpdateOrder([FromQuery] string id, [FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            OrderDto updatedOrder = await _orderService.UpdateOrderAsync(id, orderDto);
            if (updatedOrder == null)
                return NotFound(new { Message = $"Order with ID {id} not found." });

            return Ok(updatedOrder);
        }

        [HttpDelete("delete")]
        [Authorize(Roles = "Manager,CashierBA,CashierKE")]
        public async Task<IActionResult> DeleteOrder([FromQuery] string id)
        {
            bool result = await _orderService.DeleteOrderAsync(id);
            if (!result)
                return NotFound(new { Message = $"Order with ID {id} not found." });

            return NoContent();
        }
    }
}
