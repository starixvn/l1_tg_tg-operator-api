namespace TgOperator.Shared.Configuration
{
    public class MongoDbSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
        public required string VendorSettingsCollectionName { get; set; }
        public required string BetTransactionsCollectionName { get; set; }
        public required string OperatorApiLogsCollectionName { get; set; }
        //TODO: review if these are needed
        public required string PlayersCollectionName { get; set; }
        public required string MaintenanceSchedulesCollectionName { get; set; }
        public required string BetLimitsCollectionName { get; set; }
    }
} 