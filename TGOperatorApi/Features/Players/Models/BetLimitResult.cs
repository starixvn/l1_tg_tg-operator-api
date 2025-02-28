namespace TgOperator.Features.Players.Models
{
    public class BetLimitResult
    {
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
        public string Currency { get; set; }
        public string PlayerId { get; set; }
        public string Status { get; set; } = "success";
    }
} 