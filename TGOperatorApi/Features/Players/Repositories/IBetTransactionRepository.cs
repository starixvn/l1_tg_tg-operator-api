using TgOperator.Features.Players.Models;

namespace TgOperator.Features.Players.Repositories
{
    public interface IBetTransactionRepository
    {
        Task InsertAsync(BetTransaction transaction, CancellationToken cancellationToken = default);
        Task<BetTransaction?> GetByGameRoundAsync(long gameRoundId, string playerId, int gameTypeId, CancellationToken cancellationToken = default);
        Task<BetTransaction?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    }
}