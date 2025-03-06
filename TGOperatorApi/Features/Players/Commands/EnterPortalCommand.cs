using MediatR;
using TgOperator.Features.Players.Models;
using TGOperatorApi.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public record EnterPortalCommand(EnterPortalRequest Request) : IRequest<EnterPortalResponse>;
    public class EnterPortalRequest
    {
        public required string PlayerId { get; set; }
        public required string VendorId { get; set; }
        public required string SiteId { get; set; }
        public required string CharacterName { get; set; }
               
        public required string Seq { get; set; }
               
        public required bool FirstLogin { get; set; }
               
        public required string OperatorToken { get; set; }
               
        public required DateTime TimeStamp { get; set; }
    }

    public class EnterPortalResponse
    {
        public required DateTime TimeStamp { get; set; }
        public required EnterPortalResult Result { get; set; }
    }

    public class EnterPortalResult : BaseResult
    {
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
    }
} 