namespace TgOperator.Shared.Configuration
{
    public class MongoDbSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
        public required string PlayersCollectionName { get; set; }
        public required string PlayerWalletsCollectionName { get; set; }
        public string BetLimitsCollectionName { get; set; }
        public string MaintenanceSchedulesCollectionName { get; set; }
        public required string TransactionsCollectionName { get; set; }
        public required string GameTransactionsCollectionName { get; set; }
    }
} 