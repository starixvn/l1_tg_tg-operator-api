using MediatR;
using TgOperator.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public record EndRoundCommand(EndRoundRequest Request) : IRequest<EndRoundResponse>;

    public class EndRoundRequest
    {
        public required DateTime TimeStamp { get; set; }
        public required string Seq { get; set; }
        public required long GameRoundId { get; set; }
        public required int GameTypeId { get; set; }
        public required string PlayerId { get; set; }
        public required decimal TotalAmount { get; set; }
        public required decimal TotalWin { get; set; }
        public required decimal ValidBetAmount { get; set; }
        public required DateTime EndTime { get; set; }
    }

    public class EndRoundResponse
    {
        public required DateTime TimeStamp { get; set; }
        public required EndRoundResult Result { get; set; }
    }

    public class EndRoundResult
    {
        public required string Seq { get; set; }
        public required string PlayerId { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; } = "Success";
    }
} 