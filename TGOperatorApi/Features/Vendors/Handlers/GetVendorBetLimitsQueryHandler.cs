using MediatR;
using MongoDB.Driver;
using TgOperator.Features.Vendors.Models;
using TgOperator.Features.Vendors.Queries;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;

namespace TgOperator.Features.Vendors.Handlers
{
    public class GetVendorBetLimitsQueryHandler : IRequestHandler<GetVendorBetLimitsQuery, GetVendorBetLimitsResponse>
    {
        private readonly IMongoCollection<VendorSettings> _vendorSettings;

        public GetVendorBetLimitsQueryHandler(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings)
        {
            _vendorSettings = mongoDbFactory.GetCollection<VendorSettings>(
                mongoDbSettings.Value.VendorSettingsCollectionName);
        }

        public async Task<GetVendorBetLimitsResponse> Handle(GetVendorBetLimitsQuery request, CancellationToken cancellationToken)
        {
            var settings = await GetVendorSettingsAsync(request.VendorId, request.SiteId, cancellationToken);
            
            return new GetVendorBetLimitsResponse
            {
                MinBet = settings.MinBet,
                MaxBet = settings.MaxBet
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