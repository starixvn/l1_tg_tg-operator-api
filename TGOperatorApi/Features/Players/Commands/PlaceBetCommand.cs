using MediatR;
using TgOperator.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public record PlaceBetCommand(PlaceBetRequest Request) : IRequest<PlaceBetResponse>, IPlayerCommand;

    public class PlaceBetRequest
    {
        public required DateTime TimeStamp { get; set; }
        public required string Seq { get; set; }
        public required string SiteId { get; set; }
        public required long GameRoundId { get; set; }
        public required int GameTypeId { get; set; }
        public required int ChoiceId { get; set; }
        public required decimal Amount { get; set; }
        public required string Currency { get; set; }
        public required string PlayerId { get; set; }
        public required string Ip { get; set; }
        public required DateTime RoundEndTime { get; set; }
        public bool FreeSpin { get; set; }
        public bool FreeBet { get; set; }
        public decimal ContributionRate { get; set; }
        public decimal ContributionAmount { get; set; }
        public required string OperatorToken { get; set; }
    }

    public class PlaceBetResponse
    {
        public required DateTime TimeStamp { get; set; }
        public required PlaceBetResult Result { get; set; }
    }

    public class PlaceBetResult
    {
        public decimal Balance { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; } = "Success";
        public required string PlayerId { get; set; }
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
        public long ObTransId { get; set; }
    }
} 