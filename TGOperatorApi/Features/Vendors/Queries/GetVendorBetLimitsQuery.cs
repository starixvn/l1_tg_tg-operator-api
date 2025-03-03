using MediatR;

namespace TgOperator.Features.Vendors.Queries
{
    public record GetVendorBetLimitsQuery(string VendorId, string SiteId) : IRequest<GetVendorBetLimitsResponse>;

    public class GetVendorBetLimitsResponse
    {
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
    }
} 