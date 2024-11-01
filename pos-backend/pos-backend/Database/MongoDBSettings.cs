namespace pos_backend.Database
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string OrdersCollectionName { get; set; } = string.Empty;
        public string ProductsCollectionName { get; set; } = string.Empty;
        public string PaymentsCollectionName { get; set; } = string.Empty;
        public string CashRegisterCollectionName { get; set; } = string.Empty;
        public string ReceiptsCollectionName { get; set; } = string.Empty;
    }

}
