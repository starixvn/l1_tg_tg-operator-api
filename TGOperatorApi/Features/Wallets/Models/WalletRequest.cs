namespace TgOperator.Features.Wallets.Models
{
    public class DepositRequest
    {
        public string PlayerId { get; set; }
        public string OperatorId { get; set; }
        public string SiteId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentReference { get; set; }
    }

    public class WithdrawRequest
    {
        public string PlayerId { get; set; }
        public string OperatorId { get; set; }
        public string SiteId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentReference { get; set; }
    }
} 