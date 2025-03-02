using MediatR;
using TgOperator.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public record PlaceBetCommand(BetRequest BetRequest) : IRequest<BetResult>;
} 