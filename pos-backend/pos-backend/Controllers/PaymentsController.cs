using Microsoft.AspNetCore.Mvc;
using pos_backend.Models.DTOs;
using pos_backend.Services;

namespace pos_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            List<PaymentDto> payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(payments);
        }

        [HttpGet("payment")]
        public async Task<IActionResult> GetPaymentById([FromQuery] string id)
        {
            PaymentDto payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
                return NotFound(new { Message = $"Payment with ID {id} not found." });

            return Ok(payment);
        }

        [HttpPost("pay")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdPayment = await _paymentService.CreatePaymentAsync(paymentDto);
            return CreatedAtAction(nameof(GetPaymentById), new { id = createdPayment.Id }, createdPayment);
        }

        [HttpPost("refund")]
        public async Task<IActionResult> RefundPayment([FromQuery] string id, [FromBody] RefundDto? refundDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var refundedPayment = await _paymentService.RefundPaymentAsync(id, refundDto);
            if (refundedPayment == null)
                return NotFound(new { Message = $"Payment with ID {id} not found or refund not possible." });


            return Ok(refundedPayment);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePayment([FromQuery] string id)
        {
            var result = await _paymentService.DeletePaymentAsync(id);
            if (!result)
                return NotFound(new { Message = $"Payment with ID {id} not found." });

            return NoContent();
        }
    }
}
