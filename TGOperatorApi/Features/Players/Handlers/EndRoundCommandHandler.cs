using MediatR;
using MongoDB.Driver;
using TgOperator.Features.Players.Commands;
using TgOperator.Features.Players.Models;
using TgOperator.Features.Wallets.Commands;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using Sentry;

namespace TgOperator.Features.Players.Handlers
{
    public class EndRoundCommandHandler : IRequestHandler<EndRoundCommand, EndRoundResult>
    {
        private readonly IMongoCollection<GameTransaction> _gameTransactions;
        private readonly IMediator _mediator;
        private readonly IHub _sentryHub;

        public EndRoundCommandHandler(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings,
            IMediator mediator,
            IHub sentryHub)
        {
            _gameTransactions = mongoDbFactory.GetCollection<GameTransaction>(
                mongoDbSettings.Value.GameTransactionsCollectionName);
            _mediator = mediator;
            _sentryHub = sentryHub;
        }

        public async Task<EndRoundResult> Handle(EndRoundCommand command, CancellationToken cancellationToken)
        {
            var request = command.EndRoundRequest;
            var session = await _gameTransactions.Database.Client.StartSessionAsync();
            
            try
            {
                session.StartTransaction();

                // Verify if bet transaction exists
                var existingBet = await _gameTransactions
                    .Find(session, x => 
                        x.GameRoundId == request.GameRoundId && 
                        x.PlayerId == request.PlayerId.ToString() &&
                        x.Type == GameTransactionType.Bet)
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingBet == null)
                {
                    throw new InvalidOperationException("Original bet not found");
                }

                // Check for existing end round
                var existingEndRound = await _gameTransactions
                    .Find(session, x => 
                        x.GameRoundId == request.GameRoundId && 
                        x.PlayerId == request.PlayerId.ToString() &&
                        x.Type == GameTransactionType.Settle)
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingEndRound != null)
                {
                    throw new InvalidOperationException("Round already settled");
                }

                // Credit win amount to wallet
                var creditResult = await _mediator.Send(new CreditBalanceCommand(
                    new CreditBalanceRequest
                    {
                        PlayerId = request.PlayerId.ToString(),
                        VendorId = request.VendorId,
                        SiteId = request.SiteId,
                        Amount = request.TotalWin,
                        Currency = existingBet.Currency,
                        TransactionType = "win",
                        GameRoundId = request.GameRoundId
                    }), cancellationToken);

                if (!creditResult.Success)
                {
                    throw new InvalidOperationException(creditResult.Message);
                }

                // Create end round transaction
                var gameTransaction = new GameTransaction
                {
                    TransactionId = creditResult.TransactionId,
                    PlayerId = request.PlayerId.ToString(),
                    SiteId = request.SiteId,
                    Type = GameTransactionType.Settle,
                    GameRoundId = request.GameRoundId,
                    GameTypeId = request.GameTypeId,
                    Currency = existingBet.Currency,
                    TotalWin = request.TotalWin,
                    TotalAmount = request.TotalAmount,
                    ValidBetAmount = request.ValidBetAmount,
                    IsFreeBet = request.FreeBet,
                    SettleTime = request.EndTime,
                    CreatedAt = DateTime.UtcNow
                };

                await _gameTransactions.InsertOneAsync(
                    session, 
                    gameTransaction,
                    cancellationToken: cancellationToken);

                await session.CommitTransactionAsync(cancellationToken);

                return new EndRoundResult
                {
                    TransactionId = gameTransaction.TransactionId,
                    Balance = creditResult.Balance,
                    Currency = existingBet.Currency,
                    PlayerId = request.PlayerId.ToString(),
                    Status = "success",
                    Timestamp = gameTransaction.CreatedAt,
                    WinAmount = request.TotalWin,
                    GameRoundId = request.GameRoundId
                };
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync(cancellationToken);
                _sentryHub.CaptureException(ex);
                throw;
            }
            finally
            {
                session?.Dispose();
            }
        }
    }
} 