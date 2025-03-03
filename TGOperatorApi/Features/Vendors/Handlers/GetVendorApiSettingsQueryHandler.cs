using MediatR;
using MongoDB.Driver;
using TgOperator.Features.Vendors.Models;
using TgOperator.Features.Vendors.Queries;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;

namespace TgOperator.Features.Vendors.Handlers
{
    public class GetVendorApiSettingsQueryHandler : IRequestHandler<GetVendorApiSettingsQuery, GetVendorApiSettingsResponse>
    {
        private readonly IMongoCollection<VendorSettings> _vendorSettings;

        public GetVendorApiSettingsQueryHandler(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings)
        {
            _vendorSettings = mongoDbFactory.GetCollection<VendorSettings>(
                mongoDbSettings.Value.VendorSettingsCollectionName);
        }

        public async Task<GetVendorApiSettingsResponse> Handle(GetVendorApiSettingsQuery request, CancellationToken cancellationToken)
        {
            var settings = await GetVendorSettingsAsync(request.VendorId, request.SiteId, cancellationToken);
            
            return new GetVendorApiSettingsResponse
            {
                Endpoint = settings.ApiEndpoint,
                Key = settings.ApiKey,
                Secret = settings.ApiSecret
            };
        }

        private async Task<VendorSettings> GetVendorSettingsAsync(string vendorId, string siteId, CancellationToken cancellationToken)
        {
            var settings = await _vendorSettings
                .Find(x => x.VendorId == vendorId && x.SiteId == siteId)
                .FirstOrDefaultAsync(cancellationToken);

            if (settings == null)
            {
                throw new InvalidOperationException($"Vendor settings not found for vendor {vendorId} and site {siteId}");
            }

            return settings;
        }
    }
} 