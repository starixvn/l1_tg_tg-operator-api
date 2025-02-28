namespace TgOperator.Features.Wallets.Models
{
    public class WalletResponse
    {
        public string TransactionId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 