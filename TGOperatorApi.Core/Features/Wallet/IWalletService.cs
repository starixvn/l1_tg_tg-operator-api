using TgOperator.Core.Features.Wallet.Balance;

namespace TgOperator.Core.Features.Wallet
{
    public interface IWalletService
    {
        Task<GetBalanceResponse> GetBalanceAsync(string playerId, CancellationToken cancellationToken = default);
        // Future methods will be added here:
        // Task<AddBalanceResponse> AddBalanceAsync(...)
        // Task<DeductBalanceResponse> DeductBalanceAsync(...)
    }
} 