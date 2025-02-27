using TgOperator.Features.Players.Models;

namespace TgOperator.Features.Players
{
    public interface IPlayerService
    {
        Task<Player> GetPlayerBalanceAsync(string operatorId, string playerId, string siteId);
    }
} 