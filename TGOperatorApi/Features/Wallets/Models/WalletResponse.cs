namespace TgOperator.Features.Wallets.Models
{
    public class WalletResponse
    {
        public required string TransactionId { get; set; }
        public decimal Balance { get; set; }
        public required string Currency { get; set; }
        public required string Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 