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
        private readonly IMongoCollection<Player> _players;
        private readonly IMongoCollection<BetTransaction> _betTransactions;
        private readonly IHub _sentryHub;

        public PlaceBetCommandHandler(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings,
            IHub sentryHub)
        {
            _players = mongoDbFactory.GetCollection<Player>(
                mongoDbSettings.Value.PlayersCollectionName);
            _betTransactions = mongoDbFactory.GetCollection<BetTransaction>(
                mongoDbSettings.Value.BetTransactionsCollectionName);
            _sentryHub = sentryHub;
        }

        public async Task<BetResult> Handle(PlaceBetCommand command, CancellationToken cancellationToken)
        {
            var request = command.BetRequest;
            var session = await _players.Database.Client.StartSessionAsync();
            
            try
            {
                session.StartTransaction();

                var player = await _players.Find(session, x =>
                    x.OperatorId == request.OperatorId &&
                    x.PlayerId == request.PlayerId.ToString() &&
                    x.SiteId == request.SiteId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (player == null)
                {
                    throw new InvalidOperationException("Player not found");
                }

                if (player.Balance < request.Amount)
                {
                    throw new InvalidOperationException("Insufficient balance");
                }

                var transactionId = Guid.NewGuid().ToString();
                var betTransaction = new BetTransaction
                {
                    TransactionId = transactionId,
                    PlayerId = request.PlayerId,
                    OperatorId = request.OperatorId,
                    SiteId = request.SiteId,
                    Amount = request.Amount,
                    Currency = player.Currency,
                    BalanceBefore = player.Balance,
                    BalanceAfter = player.Balance - request.Amount,
                    GameRoundId = request.GameRoundId,
                    GameTypeId = request.GameTypeId,
                    Seq = request.Seq,
                    CreatedAt = DateTime.UtcNow,
                    IsFreeBet = request.FreeBet,
                    IsFreeSpin = request.FreeSpin,
                    ContributionAmount = request.ContributionAmount,
                    Status = "bet"
                };

                var filter = Builders<Player>.Filter.And(
                    Builders<Player>.Filter.Eq(x => x.OperatorId, request.OperatorId),
                    Builders<Player>.Filter.Eq(x => x.PlayerId, request.PlayerId.ToString()),
                    Builders<Player>.Filter.Eq(x => x.SiteId, request.SiteId)
                );

                var update = Builders<Player>.Update.Inc(x => x.Balance, -request.Amount);

                var updateResult = await _players.UpdateOneAsync(
                    session,
                    filter,
                    update,
                    cancellationToken: cancellationToken);

                if (updateResult.ModifiedCount != 1)
                {
                    throw new InvalidOperationException("Failed to update player balance");
                }

                await _betTransactions.InsertOneAsync(
                    session, 
                    betTransaction, 
                    cancellationToken: cancellationToken);

                await session.CommitTransactionAsync(cancellationToken);

                return new BetResult
                {
                    TransactionId = transactionId,
                    Balance = betTransaction.BalanceAfter,
                    Currency = player.Currency,
                    PlayerId = player.PlayerId,
                    Status = "success",
                    Timestamp = betTransaction.CreatedAt,
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