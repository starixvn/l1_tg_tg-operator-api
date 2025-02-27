using MongoDB.Driver;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using TgOperator.Shared.MongoDB;
using TgOperator.Features.Players.Models;
using Sentry;

namespace TgOperator.Features.Players
{
    public class PlayerService : IPlayerService
    {
        private readonly IMongoCollection<Player> _players;
        private readonly IHub _sentryHub;

        public PlayerService(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings,
            IHub sentryHub)
        {
            _players = mongoDbFactory.GetCollection<Player>(
                mongoDbSettings.Value.PlayersCollectionName);
            _sentryHub = sentryHub;
        }

        public async Task<Player> GetPlayerBalanceAsync(string operatorId, string playerId, string siteId)
        {
            try
            {
                return await _players.Find(x => 
                    x.OperatorId == operatorId && 
                    x.PlayerId == playerId && 
                    x.SiteId == siteId)
                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _sentryHub.CaptureException(ex, scope =>
                {
                    scope.SetExtra("operatorId", operatorId);
                    scope.SetExtra("playerId", playerId);
                    scope.SetExtra("siteId", siteId);
                });
                throw;
            }
        }
    }
} 