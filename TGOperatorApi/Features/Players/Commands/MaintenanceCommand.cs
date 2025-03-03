using MediatR;
using TgOperator.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public class MaintenanceCommand : IRequest<MaintenanceResult>
    {
        public MaintenanceRequest MaintenanceRequest { get; }

        public MaintenanceCommand(MaintenanceRequest request)
        {
            MaintenanceRequest = request;
        }
    }
} 