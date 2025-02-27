namespace TgOperator.Shared.Configuration
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string PlayersCollectionName { get; set; }
        public string BetTransactionsCollectionName { get; set; }
    }
} 