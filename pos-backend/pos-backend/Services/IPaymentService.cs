using pos_backend.Models.DTOs;

namespace pos_backend.Services
{
    public interface IPaymentService
    {
        Task<List<PaymentDto>> GetAllPaymentsAsync();

        Task<PaymentDto> GetPaymentByIdAsync(string id);

        Task<PaymentDto> CreatePaymentAsync(PaymentDto paymentDto);

        Task<PaymentDto> RefundPaymentAsync(string id, RefundDto refundDto);

        Task<bool> DeletePaymentAsync(string id);
    }
}
