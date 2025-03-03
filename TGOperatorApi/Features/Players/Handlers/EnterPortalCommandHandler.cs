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
    public class EnterPortalCommandHandler : IRequestHandler<EnterPortalCommand, EnterPortalResult>
    {
        private readonly IMongoCollection<Player> _players;
        private readonly IMongoCollection<BetLimit> _betLimits;
        private readonly IHub _sentryHub;

        public EnterPortalCommandHandler(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings,
            IHub sentryHub)
        {
            _players = mongoDbFactory.GetCollection<Player>(
                mongoDbSettings.Value.PlayersCollectionName);
            _betLimits = mongoDbFactory.GetCollection<BetLimit>(
                mongoDbSettings.Value.BetLimitsCollectionName);
            _sentryHub = sentryHub;
        }

        public async Task<EnterPortalResult> Handle(EnterPortalCommand command, CancellationToken cancellationToken)
        {
            var request = command.EnterPortalRequest;
            var session = await _players.Database.Client.StartSessionAsync();
            
            try
            {
                session.StartTransaction();

                var player = await _players.Find(x => x.Id == request.PlayerId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (player == null)
                {
                    return new EnterPortalResult
                    {
                        Success = false,
                        ErrorCode = 1001,
                        ErrorDescription = $"Player not found: {request.PlayerId}"
                    };
                }

                var betLimit = await _betLimits.Find(x => x.Currency == player.Currency)
                    .FirstOrDefaultAsync(cancellationToken);

                var update = Builders<Player>.Update
                    .Set(p => p.LastAccessTime, request.TimeStamp)
                    .Set(p => p.SiteId, request.SiteId);

                if (request.FirstLogin)
                {
                    update = update.Set(p => p.CharacterName, request.CharacterName);
                }

                await _players.UpdateOneAsync(
                    session,
                    p => p.Id == request.PlayerId,
                    update,
                    cancellationToken: cancellationToken);

                await session.CommitTransactionAsync(cancellationToken);
                
                return new EnterPortalResult
                {
                    TimeStamp = request.TimeStamp,
                    Success = true,
                    Seq = request.Seq,
                    ErrorCode = 0,
                    ErrorDescription = "Success",
                    MinBet = betLimit?.MinBet ?? 0,
                    MaxBet = betLimit?.MaxBet ?? decimal.MaxValue
                };
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync(cancellationToken);
                _sentryHub.CaptureException(ex);
                
                return new EnterPortalResult
                {
                    Success = false,
                    ErrorCode = 5000,
                    ErrorDescription = "Internal server error"
                };
            }
            finally
            {
                session?.Dispose();
            }
        }
    }
} 