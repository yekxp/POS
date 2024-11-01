using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using pos_backend.Database;
using pos_backend.Models;
using pos_backend.Models.DTOs;

namespace pos_backend.Services.Impl
{
    public class CashRegisterService : ICashRegisterService
    {
        private readonly IMongoCollection<CashRegister> _cashRegisterCollection;
        private readonly IMongoCollection<Receipt> _receiptsCollection;
        private readonly IMongoCollection<Payment> _paymentsCollection;
        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly IMapper _mapper;

        public CashRegisterService(IOptions<MongoDBSettings> settings, IMongoClient mongoClient, IMapper mapper)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _cashRegisterCollection = database.GetCollection<CashRegister>(settings.Value.CashRegisterCollectionName);
            _receiptsCollection = database.GetCollection<Receipt>(settings.Value.ReceiptsCollectionName);
            _paymentsCollection = database.GetCollection<Payment>(settings.Value.PaymentsCollectionName);
            _ordersCollection = database.GetCollection<Order>(settings.Value.OrdersCollectionName);
            _mapper = mapper;
        }

        public async Task<List<CashRegisterDto>> GetAllRegistersAsync()
        {
            List<CashRegister> registers = await _cashRegisterCollection.Find(_ => true).ToListAsync();
            return _mapper.Map<List<CashRegisterDto>>(registers);
        }

        public async Task<CashRegisterDto> GetRegisterByIdAsync(string id)
        {
            CashRegister register = await _cashRegisterCollection.Find(r => r.Id == id).FirstOrDefaultAsync();
            return register == null ? null : _mapper.Map<CashRegisterDto>(register);
        }

        public async Task<bool> OpenRegisterAsync(CashRegisterOpenDto registerOpenDto)
        {
            CashRegister openRegister = await _cashRegisterCollection.Find(r => r.IsOpen && r.Location == registerOpenDto.Location).FirstOrDefaultAsync();
            if (openRegister != null)
                return false;

            CashRegister newRegister = new CashRegister
            {
                InitialCash = registerOpenDto.InitialCash,
                TotalCash = registerOpenDto.InitialCash,
                IsOpen = true,
                Location = registerOpenDto.Location,
                OpenedAt = DateTime.UtcNow
            };

            await _cashRegisterCollection.InsertOneAsync(newRegister);
            return true;
        }

        public async Task<bool> CloseRegisterAsync(Location location)
        {
            CashRegister openRegister = await _cashRegisterCollection.Find(r => r.IsOpen && r.Location == location).FirstOrDefaultAsync();
            if (openRegister == null) return false;

            openRegister.IsOpen = false;
            openRegister.ClosedAt = DateTime.UtcNow;

            FilterDefinition<Payment> filter = Builders<Payment>.Filter.And(
                Builders<Payment>.Filter.Eq(p => p.Status, PaymentStatus.Completed),
                Builders<Payment>.Filter.Gte(p => p.PaymentDate, openRegister.OpenedAt),
                Builders<Payment>.Filter.Lt(p => p.PaymentDate, openRegister.ClosedAt)
            );

            List<Payment> totalAmount = await _paymentsCollection
                .Find(filter)
                .ToListAsync();

            decimal totalCashAmount = totalAmount.Sum(p => p.Amount);
            openRegister.TotalCash += totalCashAmount;
 
            ReplaceOneResult result = await _cashRegisterCollection.ReplaceOneAsync(r => r.Id == openRegister.Id, openRegister);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> CashInAsync(CashInDto cashInDto)
        {
            CashRegister openRegister = await _cashRegisterCollection.Find(r => r.IsOpen).FirstOrDefaultAsync();
            if (openRegister == null) return false;

            openRegister.TotalCash += cashInDto.Amount;

            var result = await _cashRegisterCollection.ReplaceOneAsync(r => r.Id == openRegister.Id, openRegister);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> CashOutAsync(CashOutDto cashOutDto)
        {
            CashRegister openRegister = await _cashRegisterCollection.Find(r => r.IsOpen).FirstOrDefaultAsync();
            if (openRegister == null || openRegister.TotalCash < cashOutDto.Amount)
                return false;

            openRegister.TotalCash -= cashOutDto.Amount;

            var result = await _cashRegisterCollection.ReplaceOneAsync(r => r.Id == openRegister.Id, openRegister);
            return result.ModifiedCount > 0;
        }

        public async Task<ReceiptDto> GenerateReceiptAsync(ReceiptDto receiptDto)
        {
            Payment existingPayment = await _paymentsCollection.Find(p => p.Id == receiptDto.PaymentId).FirstOrDefaultAsync();
            if (existingPayment == null)
                throw new Exception($"Payment with ID {receiptDto.Id} does not exist.");

            Receipt newReceipt = _mapper.Map<Receipt>(receiptDto);
            newReceipt.Date = DateTime.UtcNow;

            Order paymentOrder = await _ordersCollection.Find(p => p.Id == existingPayment.OrderId).FirstOrDefaultAsync();

            newReceipt.TotalAmount = paymentOrder.Items.Sum(x => x.Quantity * x.Price);

            if (newReceipt.CashPaid < newReceipt.TotalAmount)
                throw new Exception($"Insufficient cash paid. Cash Paid: {newReceipt.CashPaid}, Total Amount: {newReceipt.TotalAmount}.");
  

            newReceipt.ChangeGiven = newReceipt.CashPaid - newReceipt.TotalAmount;
            newReceipt.Items = paymentOrder.Items;

            await _receiptsCollection.InsertOneAsync(newReceipt);

            return _mapper.Map<ReceiptDto>(newReceipt);
        }

        public async Task<bool> DeleteReceiptAsync(string receiptId)
        {
            var deleteResult = await _receiptsCollection.DeleteOneAsync(r => r.Id == receiptId);
            return deleteResult.DeletedCount > 0;
        }
    }
}
