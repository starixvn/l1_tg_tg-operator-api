using TgOperator.Features.Players.Models;

public partial interface ITgOperatorClient
{
    Task<Player> GetPlayerBalanceAsync(PlayerBalanceRequest request, CancellationToken cancellationToken = default);
    Task<BetResult> PlaceBetAsync(BetRequest request, CancellationToken cancellationToken = default);
    Task<EndRoundResult> EndRoundAsync(EndRoundRequest request, CancellationToken cancellationToken = default);
    Task<VoidGameResult> VoidGameAsync(VoidGameRequest request, CancellationToken cancellationToken = default);
    Task<BetLimitResult> GetBetLimitAsync(BetLimitRequest request, CancellationToken cancellationToken = default);
}