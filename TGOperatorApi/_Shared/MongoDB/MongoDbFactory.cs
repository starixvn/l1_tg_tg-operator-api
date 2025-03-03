using MongoDB.Driver;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;

namespace TgOperator.Shared.MongoDB
{
    public interface IMongoDbFactory
    {
        IMongoDatabase GetDatabase();
        IMongoCollection<T> GetCollection<T>(string collectionName);
        IMongoCollection<T> GetCollection<T>();
    }

    public class MongoDbFactory : IMongoDbFactory
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDbSettings _settings;

        public MongoDbFactory(IOptions<MongoDbSettings> settings)
        {
            _settings = settings.Value;
            var client = new MongoClient(_settings.ConnectionString);
            _database = client.GetDatabase(_settings.DatabaseName);
        }

        public IMongoDatabase GetDatabase() => _database;

        public IMongoCollection<T> GetCollection<T>(string collectionName)
            => _database.GetCollection<T>(collectionName);

        public IMongoCollection<T> GetCollection<T>()
            => GetCollection<T>(GetDefaultCollectionName<T>());

        private string GetDefaultCollectionName<T>()
        {
            var type = typeof(T);
            return type.Name switch
            {
                "BetTransaction" => _settings.BetTransactionsCollectionName,
                "OperatorApiLog" => _settings.OperatorApiLogsCollectionName,
                "VendorSettings" => _settings.VendorSettingsCollectionName,
                _ => throw new ArgumentException($"No default collection name defined for type {type.Name}")
            };
        }
    }
} 