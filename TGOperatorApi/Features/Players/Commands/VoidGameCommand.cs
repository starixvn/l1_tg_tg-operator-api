using MediatR;
using TgOperator.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public class VoidGameCommand : IRequest<VoidGameResult>
    {
        public VoidGameRequest VoidGameRequest { get; }

        public VoidGameCommand(VoidGameRequest voidGameRequest)
        {
            VoidGameRequest = voidGameRequest;
        }
    }
} 