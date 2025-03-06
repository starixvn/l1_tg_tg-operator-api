using MediatR;
using TgOperator.Features.Players.Models;
using TGOperatorApi.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public record VoidGameCommand(VoidGameRequest Request) : IRequest<VoidGameResponse>;

    public class VoidGameRequest
    {
        public required string Seq { get; set; }
        public required string OperatorId { get; set; }

        public required string PlayerId { get; set; }

        public required string SiteId { get; set; }

        public required int GameRoundId { get; set; }

        public required byte GameTypeId { get; set; }

        public required string Reason { get; set; }

        public required bool FreeBet { get; set; }
    }

    public class VoidGameResponse
    {
        public required DateTime TimeStamp { get; set; }
        public required VoidGameResult Result { get; set; }
    }

    public class VoidGameResult : BaseResult
    {
        public string TransactionId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public string PlayerId { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Amount { get; set; }
        public long GameRoundId { get; set; }
    }
} 