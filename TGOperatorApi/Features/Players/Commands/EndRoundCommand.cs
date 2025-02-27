using MediatR;
using TgOperator.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public class EndRoundCommand : IRequest<EndRoundResult>
    {
        public EndRoundRequest EndRoundRequest { get; }

        public EndRoundCommand(EndRoundRequest endRoundRequest)
        {
            EndRoundRequest = endRoundRequest;
        }
    }
} 