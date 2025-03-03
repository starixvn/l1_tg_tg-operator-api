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
    public class VoidGameCommandHandler : IRequestHandler<VoidGameCommand, VoidGameResult>
    {
        private readonly IMongoCollection<Player> _players;
        private readonly IMongoCollection<GameTransaction> _betTransactions;
        private readonly IHub _sentryHub;

        public VoidGameCommandHandler(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings,
            IHub sentryHub)
        {
            _players = mongoDbFactory.GetCollection<Player>(
                mongoDbSettings.Value.PlayersCollectionName);
            _betTransactions = mongoDbFactory.GetCollection<GameTransaction>(
                mongoDbSettings.Value.GameTransactionsCollectionName);
            _sentryHub = sentryHub;
        }

        public async Task<VoidGameResult> Handle(VoidGameCommand command, CancellationToken cancellationToken)
        {
            var request = command.VoidGameRequest;
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

                var betTransaction = await _betTransactions.Find(session, x =>
                    x.GameRoundId == request.GameRoundId &&
                    x.PlayerId == request.PlayerId &&
                    x.GameTypeId == request.GameTypeId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (betTransaction == null)
                {
                    throw new InvalidOperationException("Bet transaction not found");
                }

                var updatePlayerBalance = Builders<Player>.Update
                    .Inc(x => x.Balance, betTransaction.TotalAmount ?? 0);
                
                await _players.UpdateOneAsync(session,
                    x => x.Id == player.Id,
                    updatePlayerBalance);

                var transactionId = Guid.NewGuid().ToString();
                var voidTransaction = new GameTransaction
                {
                    Id = transactionId,
                    TransactionId = transactionId,
                    PlayerId = player.PlayerId,                    
                    SiteId = player.SiteId,
                    Currency = player.Currency,
                    GameRoundId = request.GameRoundId,
                    GameTypeId = request.GameTypeId,
                    Type = GameTransactionType.Void,
                    CreatedAt = DateTime.UtcNow
                };

                await _betTransactions.InsertOneAsync(session, voidTransaction);

                await session.CommitTransactionAsync(cancellationToken);

                return new VoidGameResult
                {
                    TransactionId = transactionId,
                    Currency = player.Currency,
                    PlayerId = player.PlayerId,
                    Timestamp = voidTransaction.CreatedAt,
                    GameRoundId = request.GameRoundId,
                    Balance = player.Balance, // TODO: get balance after void
                    Success = true,
                    ErrorCode = 0
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