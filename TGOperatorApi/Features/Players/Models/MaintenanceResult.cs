using TGOperatorApi.Features.Players.Models;

namespace TgOperator.Features.Players.Models
{
    public class MaintenanceResult : BaseResult
    {
        public bool IsMaintenance { get; set; }
        public string Message { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
} 