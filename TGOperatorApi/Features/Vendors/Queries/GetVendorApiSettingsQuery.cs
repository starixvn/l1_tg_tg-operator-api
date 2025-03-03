using MediatR;

namespace TgOperator.Features.Vendors.Queries
{
    public record GetVendorApiSettingsQuery(string VendorId, string SiteId) : IRequest<GetVendorApiSettingsResponse>;

    public class GetVendorApiSettingsResponse
    {
        public required string Endpoint { get; set; }
        public required string Key { get; set; }
        public required string Secret { get; set; }
    }
} 