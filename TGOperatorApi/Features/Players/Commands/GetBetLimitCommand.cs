using MediatR;
using TgOperator.Features.Players.Models;
using TGOperatorApi.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public record GetBetLimitCommand(BetLimitRequest Request) : IRequest<BetLimitResponse>;

    public class BetLimitRequest
    {
        public required string Seq { get; set; }
        public required string PlayerId { get; set; }
        public required string VendorId { get; set; }
        public required string SiteId { get; set; }
        public required string GameTypeId { get; set; }
        public required string Currency { get; set; }
    }

    public class BetLimitResponse
    {
        public required DateTime TimeStamp { get; set; }
        public required BetLimitResult Result { get; set; }
    }

    public class BetLimitResult : BaseResult
    {
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
    }
} 