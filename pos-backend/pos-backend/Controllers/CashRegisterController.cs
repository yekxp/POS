using Microsoft.AspNetCore.Mvc;
using pos_backend.Models;
using pos_backend.Models.DTOs;
using pos_backend.Services;

namespace pos_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CashRegisterController : ControllerBase
    {
        private readonly ICashRegisterService _cashRegisterService;

        public CashRegisterController(ICashRegisterService cashRegisterService)
        {
            _cashRegisterService = cashRegisterService;
        }

        [HttpPost("open")]
        public async Task<IActionResult> OpenRegister([FromBody] CashRegisterOpenDto openDto)
        {
            var result = await _cashRegisterService.OpenRegisterAsync(openDto);
            if (!result)
                return BadRequest("Failed to open the cash register. Another register might be open.");

            return Ok("Cash register opened successfully.");
        }

        [HttpPost("close")]
        public async Task<IActionResult> CloseRegister([FromQuery] Location location)
        {
            var result = await _cashRegisterService.CloseRegisterAsync(location);
            if (!result)
                return BadRequest("Failed to close the cash register. No open register found.");

            return Ok("Cash register closed successfully.");
        }

        [HttpPost("cashin")]
        public async Task<IActionResult> CashIn([FromBody] CashInDto cashInDto)
        {
            var result = await _cashRegisterService.CashInAsync(cashInDto);
            if (!result)
                return BadRequest("Failed to add cash to the register. Make sure the register is open.");

            return Ok("Cash added to the register successfully.");
        }

        [HttpPost("cashout")]
        public async Task<IActionResult> CashOut([FromBody] CashOutDto cashOutDto)
        {
            var result = await _cashRegisterService.CashOutAsync(cashOutDto);
            if (!result)
                return BadRequest("Failed to remove cash from the register. Make sure the register is open and has sufficient cash.");

            return Ok("Cash removed from the register successfully.");
        }

        [HttpPost("receipt")]
        public async Task<ActionResult<ReceiptDto>> GenerateReceipt([FromBody] ReceiptDto receiptDto)
        {
            var receipt = await _cashRegisterService.GenerateReceiptAsync(receiptDto);
            if (receipt == null)
                return BadRequest("Failed to generate receipt. Make sure the register is open.");

            return Ok(receipt);
        }
    }
}
