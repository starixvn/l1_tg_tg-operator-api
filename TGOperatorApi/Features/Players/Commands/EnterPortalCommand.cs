using MediatR;
using TgOperator.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public class EnterPortalCommand : IRequest<EnterPortalResult>
    {
        public EnterPortalRequest EnterPortalRequest { get; }

        public EnterPortalCommand(EnterPortalRequest request)
        {
            EnterPortalRequest = request;
        }
    }
} 