using MediatR;
using TgOperator.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public class GetBetLimitCommand : IRequest<BetLimitResult>
    {
        public BetLimitRequest BetLimitRequest { get; }

        public GetBetLimitCommand(BetLimitRequest betLimitResult)
        {
            BetLimitRequest = betLimitResult;
        }
    }
} 