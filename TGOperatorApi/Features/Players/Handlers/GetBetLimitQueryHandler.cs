using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TgOperator.Features.Players.Commands;
using TgOperator.Features.Players.Models;
using TgOperator.Shared.Configuration;
using TgOperator.Shared.MongoDB;

namespace TgOperator.Features.Players.Handlers
{
    public class GetBetLimitQueryHandler : IRequestHandler<GetBetLimitCommand, BetLimitResult>
    {
        private readonly IMongoCollection<Player> _players;
        private readonly IMongoCollection<BetLimit> _betLimits;
        private readonly IHub _sentryHub;

        public GetBetLimitQueryHandler(
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

        public async Task<BetLimitResult> Handle(GetBetLimitCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var request = command.BetLimitRequest;
                
                var player = await _players.Find(x =>
                    x.OperatorId == request.OperatorId &&
                    x.PlayerId == request.PlayerId.ToString() &&
                    x.SiteId == request.SiteId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (player == null)
                {
                    throw new InvalidOperationException("Player not found");
                }

                var betLimit = await _betLimits.Find(x =>
                    x.Currency == request.Currency)
                    .FirstOrDefaultAsync(cancellationToken);

                if (betLimit == null)
                {
                    throw new InvalidOperationException("Bet limit configuration not found");
                }

                var betLimitResult = new BetLimitResult
                {
                    PlayerId = player.PlayerId,
                    Currency = player.Currency,
                    MaxBet = betLimit.MaxBet,
                    MinBet = betLimit.MinBet,
                    Status = "success",
                };

                return betLimitResult;
            }
            catch (Exception ex)
            {
                _sentryHub.CaptureException(ex);
                throw;
            }
        }
    }
} 