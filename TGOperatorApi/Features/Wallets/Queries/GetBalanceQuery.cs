using MediatR;

namespace TgOperator.Features.Wallets.Queries
{
    public record GetBalanceQuery(string PlayerId, string SiteId) : IRequest<GetBalanceResponse>;

    public class GetBalanceResponse
    {
        public required string PlayerId { get; set; }
        public required string SiteId { get; set; }
        public decimal Balance { get; set; }
        public required string Currency { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
} 