using pos_backend.Models;
using pos_backend.Models.DTOs;

namespace pos_backend.Services
{
    public interface ICashRegisterService
    {
        Task<bool> OpenRegisterAsync(CashRegisterOpenDto registerOpenDto);
        Task<bool> CloseRegisterAsync(Location location);
        Task<bool> CashInAsync(CashInDto cashInDto);
        Task<bool> CashOutAsync(CashOutDto cashOutDto);
        Task<ReceiptDto> GenerateReceiptAsync(ReceiptDto receiptDto);
    }
}
