using MediatR;
using MongoDB.Driver;
using TgOperator.Features.Players.Commands;
using TgOperator.Features.Players.Models;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using Sentry;

namespace TgOperator.Features.Players.Handlers
{
    public class PlaceBetCommandHandler : IRequestHandler<PlaceBetCommand, BetResult>
    {
        private readonly IMongoCollection<GameTransaction> _gameTransactions;
        private readonly IMediator _mediator;
        private readonly IHub _sentryHub;

        public PlaceBetCommandHandler(
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

        public async Task<BetResult> Handle(PlaceBetCommand command, CancellationToken cancellationToken)
        {
            var request = command.BetRequest;
            var session = await _gameTransactions.Database.Client.StartSessionAsync();
            
            try
            {
                session.StartTransaction();

                // TODO: Deduct balance from wallet
                
                // Create game transaction
                var gameTransaction = new GameTransaction
                {
                    TransactionId = deductResult.TransactionId,
                    PlayerId = request.PlayerId.ToString(),
                    SiteId = request.SiteId,
                    Type = GameTransactionType.Bet,
                    GameRoundId = request.GameRoundId,
                    GameTypeId = request.GameTypeId,
                    Currency = request.Currency,
                    BetAmount = request.Amount,
                    IsFreeBet = request.FreeBet,
                    IsFreeSpin = request.FreeSpin,
                    ContributionAmount = request.ContributionAmount,
                    CreatedAt = DateTime.UtcNow
                };

                await _gameTransactions.InsertOneAsync(
                    session, 
                    gameTransaction,
                    cancellationToken: cancellationToken);

                await session.CommitTransactionAsync(cancellationToken);

                return new BetResult
                {
                    TransactionId = gameTransaction.TransactionId,
                    Balance = deductResult.Balance,
                    Currency = request.Currency,
                    PlayerId = request.PlayerId.ToString(),
                    Status = "success",
                    Timestamp = gameTransaction.CreatedAt,
                    Amount = request.Amount,
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