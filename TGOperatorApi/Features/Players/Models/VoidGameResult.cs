namespace TgOperator.Features.Players.Models
{
    public class VoidGameResult
    {
        public string TransactionId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public string PlayerId { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Amount { get; set; }
        public long GameRoundId { get; set; }
    }
} 