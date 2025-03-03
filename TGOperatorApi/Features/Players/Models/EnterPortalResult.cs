using TGOperatorApi.Features.Players.Models;

namespace TgOperator.Features.Players.Models
{
    public class EnterPortalResult : BaseResult
    {
        public DateTime TimeStamp { get; set; }
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
    }
} 