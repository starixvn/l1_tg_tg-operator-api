namespace TgOperator.Features.Players.Models
{
    public class MaintenanceResult
    {
        public bool IsMaintenance { get; set; }
        public string Message { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = "success";
    }
} 