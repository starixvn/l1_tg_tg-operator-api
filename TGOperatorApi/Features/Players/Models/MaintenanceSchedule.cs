namespace TgOperator.Features.Players.Models
{
    public class MaintenanceSchedule
    {
        public string Id { get; set; }
        public string OperatorId { get; set; }
        public string SiteId { get; set; }
        public string GameTypeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 