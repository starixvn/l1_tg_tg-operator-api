using TGOperatorApi.Features.Players.Models;

namespace TgOperator.Features.Players.Models
{
    public class BetLimitResult : BaseResult
    {
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
        public string Currency { get; set; }
        public string PlayerId { get; set; }
    }
} 