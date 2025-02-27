using MediatR;
using TgOperator.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public class PlaceBetCommand : IRequest<BetResult>
    {
        public BetRequest BetRequest { get; }

        public PlaceBetCommand(BetRequest betRequest)
        {
            BetRequest = betRequest;
        }
    }
} 