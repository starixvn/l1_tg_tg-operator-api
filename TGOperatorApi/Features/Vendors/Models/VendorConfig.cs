namespace TGOperatorApi.Features.Vendors.Models
{
    public record VendorConfig
    {
        public string ApiUrl { get; init; }
        public string OperatorId { get; init; }
        public string OperatorSecret { get; init; }
        public string OperatorToken { get; init; }
        public string SiteId { get; init; }
        public string SiteName { get; init; }
        public string Ref { get; init; }
    }
}
