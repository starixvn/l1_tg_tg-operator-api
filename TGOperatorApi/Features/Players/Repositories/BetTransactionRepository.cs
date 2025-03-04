using MongoDB.Driver;
using TgOperator.Features.Players.Models;
using TgOperator.Shared.MongoDB;

namespace TgOperator.Features.Players.Repositories
{
    public class BetTransactionRepository : IBetTransactionRepository
    {
        private readonly IMongoCollection<BetTransaction> _betTransactions;

        public BetTransactionRepository(IMongoDbFactory mongoDbFactory)
        {
            _betTransactions = mongoDbFactory.GetCollection<BetTransaction>();
        }

        public async Task InsertAsync(BetTransaction transaction, CancellationToken cancellationToken = default)
        {
            await _betTransactions.InsertOneAsync(
                transaction,
                new InsertOneOptions(),
                cancellationToken);
        }

        public async Task<BetTransaction?> GetByGameRoundAsync(long gameRoundId, string playerId, int gameTypeId, CancellationToken cancellationToken = default)
        {
            return await _betTransactions
                .Find(x => 
                    x.GameRoundId == gameRoundId && 
                    x.PlayerId == playerId && 
                    x.GameTypeId == gameTypeId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<BetTransaction?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
        {
            return await _betTransactions
                .Find(x => x.TransactionId == transactionId)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
} 