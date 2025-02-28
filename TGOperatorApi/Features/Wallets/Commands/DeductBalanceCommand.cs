using MediatR;

namespace TgOperator.Features.Wallets.Commands
{
    public record DeductBalanceCommand(DeductBalanceRequest DeductRequest) : IRequest<DeductBalanceResult>;

    public class DeductBalanceRequest
    {
        public string PlayerId { get; set; }
        public string OperatorId { get; set; }
        public string SiteId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string TransactionType { get; set; }
        public long GameRoundId { get; set; }
    }

    public class DeductBalanceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public decimal Balance { get; set; }
    }
} 