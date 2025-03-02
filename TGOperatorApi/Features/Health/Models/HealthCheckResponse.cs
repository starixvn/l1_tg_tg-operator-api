namespace TgOperator.Features.Health.Models
{
    public class HealthCheckResponse
    {
        public required string Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 