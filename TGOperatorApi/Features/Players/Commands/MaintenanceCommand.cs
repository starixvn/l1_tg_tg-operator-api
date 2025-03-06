using MediatR;
using TgOperator.Features.Players.Models;
using TGOperatorApi.Features.Players.Models;

namespace TgOperator.Features.Players.Commands
{
    public record MaintenanceCommand(MaintenanceRequest Request) : IRequest<MaintenanceResponse>;

    public class MaintenanceRequest
    {
        public required string Seq { get; set; }
        public required string OperatorId { get; set; }
        public required string SiteId { get; set; }
        public required string GameTypeId { get; set; }
    }

    public class MaintenanceResponse
    {
        public required DateTime TimeStamp { get; set; }
        public required MaintenanceResult Result { get; set; }
    }

    public class MaintenanceResult : BaseResult
    {
        public bool IsMaintenance { get; set; }
        public string Message { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
} 