namespace TgOperator.Shared.Configuration
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string PlayersCollectionName { get; set; }
        public string PlayerWalletsCollectionName { get; set; }
        public string TransactionsCollectionName { get; set; }
        public string GameTransactionsCollectionName { get; set; }
    }
} 