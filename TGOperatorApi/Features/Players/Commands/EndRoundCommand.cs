using MediatR;
using TgOperator.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public record EndRoundCommand(EndRoundRequest EndRoundRequest) : IRequest<EndRoundResult>;
} 