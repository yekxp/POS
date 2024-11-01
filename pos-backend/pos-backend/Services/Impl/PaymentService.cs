using AutoMapper;
using MongoDB.Driver;
using pos_backend.Models.DTOs;
using pos_backend.Models;
using Microsoft.Extensions.Options;
using pos_backend.Database;

namespace pos_backend.Services.Impl
{
    public class PaymentService : IPaymentService
    {
        private readonly IMongoCollection<Payment> _paymentsCollection;
        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly IMapper _mapper;

        public PaymentService(IOptions<MongoDBSettings> settings, IMongoClient mongoClient, IMapper mapper)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _ordersCollection = database.GetCollection<Order>(settings.Value.OrdersCollectionName);
            _paymentsCollection = database.GetCollection<Payment>(settings.Value.PaymentsCollectionName);

            _mapper = mapper;
        }

        public async Task<List<PaymentDto>> GetAllPaymentsAsync()
        {
            List<Payment> payments = await _paymentsCollection.Find(_ => true).ToListAsync();

            return _mapper.Map<List<PaymentDto>>(payments);
        }

        public async Task<PaymentDto> GetPaymentByIdAsync(string id)
        {
            Payment payment = await _paymentsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();

            if (payment == null) 
                return null;

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto> CreatePaymentAsync(PaymentDto paymentDto)
        {
            Payment payment = _mapper.Map<Payment>(paymentDto);

            Order order = await _ordersCollection.Find(o => o.Id == payment.OrderId).FirstOrDefaultAsync()
                ?? throw new Exception($"Order with ID {payment.OrderId} not found.");

            payment.Amount = order.TotalAmount;
            payment.PaymentDate = DateTime.UtcNow;
            payment.Status = PaymentStatus.Completed;

            await _paymentsCollection.InsertOneAsync(payment);

            order.Status = PaymentStatus.Completed;

            await _ordersCollection.ReplaceOneAsync(o => o.Id == order.Id, order);

            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto> RefundPaymentAsync(string id, RefundDto refundDto)
        {
            Payment existingPayment = await _paymentsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (existingPayment == null || existingPayment.Status != PaymentStatus.Completed)
                return null;

            existingPayment.Status = PaymentStatus.Refunded;
            existingPayment.RefundAmount = existingPayment.Amount;
            existingPayment.RefundReason = refundDto.Reason;
            existingPayment.RefundDate = refundDto.RefundDate;

            await _paymentsCollection.ReplaceOneAsync(p => p.Id == id, existingPayment);

            Order order = await _ordersCollection.Find(o => o.Id == existingPayment.OrderId).FirstOrDefaultAsync();
            if (order != null)
            {
                order.Status = PaymentStatus.Cancelled;
                order.TotalAmount = 0;
                await _ordersCollection.ReplaceOneAsync(o => o.Id == order.Id, order);
            }

            return _mapper.Map<PaymentDto>(existingPayment);
        }


        public async Task<bool> DeletePaymentAsync(string id)
        {
            Payment existingPayment = await _paymentsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (existingPayment == null)
                return false;

            DeleteResult result = await _paymentsCollection.DeleteOneAsync(p => p.Id == id);

            Order order = await _ordersCollection.Find(o => o.Id == existingPayment.OrderId).FirstOrDefaultAsync();
            if (order != null)
            {
                order.Status = PaymentStatus.Cancelled;
                await _ordersCollection.ReplaceOneAsync(o => o.Id == order.Id, order);
            }

            return result.DeletedCount > 0;
        }
    }

}

