namespace TgOperator.Features.Players.Models
{
    public class EndRoundResult
    {
        public required string TransactionId { get; set; }
        public decimal Balance { get; set; }
        public required string Currency { get; set; }
        public required string PlayerId { get; set; }
        public required string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal WinAmount { get; set; }
        public long GameRoundId { get; set; }
    }
} 