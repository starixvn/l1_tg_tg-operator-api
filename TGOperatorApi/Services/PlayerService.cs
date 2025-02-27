using MongoDB.Driver;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using TgOperator.Shared.MongoDB;

public interface IPlayerService
{
    Task<Player> GetPlayerBalanceAsync(string operatorId, string playerId, string siteId);
}

public class PlayerService : IPlayerService
{
    private readonly IMongoCollection<Player> _players;

    public PlayerService(
        IMongoDbFactory mongoDbFactory,
        IOptions<MongoDbSettings> mongoDbSettings)
    {
        _players = mongoDbFactory.GetCollection<Player>(
            mongoDbSettings.Value.PlayersCollectionName);
    }

    public async Task<Player> GetPlayerBalanceAsync(string operatorId, string playerId, string siteId)
    {
        return await _players.Find(x => 
            x.OperatorId == operatorId && 
            x.PlayerId == playerId && 
            x.SiteId == siteId)
        .FirstOrDefaultAsync();
    }
} 