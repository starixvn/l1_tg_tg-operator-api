using MediatR;

namespace TgOperator.Features.Wallets.Commands
{
    public record CreditBalanceCommand(CreditBalanceRequest CreditRequest) : IRequest<CreditBalanceResult>;

    public class CreditBalanceRequest
    {
        public string PlayerId { get; set; }
        public string VendorId { get; set; }
        public string SiteId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string TransactionType { get; set; }
        public long GameRoundId { get; set; }
    }

    public class CreditBalanceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public decimal Balance { get; set; }
    }
} 