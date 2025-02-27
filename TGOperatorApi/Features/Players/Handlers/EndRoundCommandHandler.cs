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
    public class EndRoundCommandHandler : IRequestHandler<EndRoundCommand, EndRoundResult>
    {
        private readonly IMongoCollection<Player> _players;
        private readonly IMongoCollection<BetTransaction> _betTransactions;
        private readonly IHub _sentryHub;

        public EndRoundCommandHandler(
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

        public async Task<EndRoundResult> Handle(EndRoundCommand command, CancellationToken cancellationToken)
        {
            var request = command.EndRoundRequest;
            var session = await _players.Database.Client.StartSessionAsync();
            
            try
            {
                session.StartTransaction();

                // Verify if bet transaction exists and hasn't been settled
                var existingBet = await _betTransactions
                    .Find(session, x => 
                        x.GameRoundId == request.GameRoundId && 
                        x.PlayerId == request.PlayerId &&
                        x.Status == TransactionType.Bet.ToString().ToLower())
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingBet == null)
                {
                    throw new InvalidOperationException("Original bet not found");
                }

                // Check for existing settlement to prevent double settlement
                var existingSettle = await _betTransactions
                    .Find(session, x => 
                        x.GameRoundId == request.GameRoundId && 
                        x.PlayerId == request.PlayerId &&
                        x.Status == TransactionType.Settle.ToString().ToLower())
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingSettle != null)
                {
                    throw new InvalidOperationException("Round already settled");
                }

                // Find player
                var player = await _players
                    .Find(session, x =>
                        x.PlayerId == request.PlayerId.ToString() &&
                        x.SiteId == request.SiteId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (player == null)
                {
                    throw new InvalidOperationException("Player not found");
                }

                // Create settlement transaction
                var transactionId = Guid.NewGuid().ToString();
                var settleTransaction = new BetTransaction
                {
                    TransactionId = transactionId,
                    PlayerId = request.PlayerId,
                    OperatorId = player.OperatorId,
                    SiteId = request.SiteId,
                    Amount = request.TotalWin,
                    Currency = player.Currency,
                    BalanceBefore = player.Balance,
                    BalanceAfter = player.Balance + request.TotalWin,
                    GameRoundId = request.GameRoundId,
                    GameTypeId = request.GameTypeId,
                    Seq = request.Seq,
                    CreatedAt = DateTime.UtcNow,
                    IsFreeBet = request.FreeBet,
                    IsFreeSpin = false,
                    ContributionAmount = 0,
                    Status = TransactionType.Settle.ToString().ToLower()
                };

                // Update player's balance
                var filter = Builders<Player>.Filter.And(
                    Builders<Player>.Filter.Eq(x => x.PlayerId, request.PlayerId.ToString()),
                    Builders<Player>.Filter.Eq(x => x.SiteId, request.SiteId)
                );

                var update = Builders<Player>.Update.Inc(x => x.Balance, request.TotalWin);

                var updateResult = await _players.UpdateOneAsync(
                    session,
                    filter,
                    update,
                    cancellationToken: cancellationToken);

                if (updateResult.ModifiedCount != 1)
                {
                    throw new InvalidOperationException("Failed to update player balance");
                }

                // Save transaction record
                await _betTransactions.InsertOneAsync(
                    session, 
                    settleTransaction,
                    cancellationToken: cancellationToken);

                // Update original bet transaction status
                var betFilter = Builders<BetTransaction>.Filter.Eq(x => x.Id, existingBet.Id);
                var betUpdate = Builders<BetTransaction>.Update
                    .Set(x => x.Status, TransactionType.Settle.ToString().ToLower());

                await _betTransactions.UpdateOneAsync(
                    session,
                    betFilter,
                    betUpdate,
                    cancellationToken: cancellationToken);

                await session.CommitTransactionAsync(cancellationToken);

                return new EndRoundResult
                {
                    TransactionId = transactionId,
                    Balance = settleTransaction.BalanceAfter,
                    Currency = player.Currency,
                    PlayerId = player.PlayerId,
                    Status = "success",
                    Timestamp = settleTransaction.CreatedAt,
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