using MongoDB.Driver;
using TgOperator.Features.Vendors.Models;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;

namespace TgOperator.Features.Vendors.Services
{
    public interface IVendorSettingsService
    {
        Task<ApiSettings> GetApiSettingsAsync(string vendorId, string siteId);
        Task<BetLimits> GetBetLimitsAsync(string vendorId, string siteId);
        Task<bool> ValidateVendorAsync(string vendorId, string siteId);
    }

    public class VendorSettingsService : IVendorSettingsService
    {
        private readonly IMongoCollection<VendorSettings> _vendorSettings;

        public VendorSettingsService(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings)
        {
            _vendorSettings = mongoDbFactory.GetCollection<VendorSettings>(
                mongoDbSettings.Value.VendorSettingsCollectionName);
        }

        public async Task<ApiSettings> GetApiSettingsAsync(string vendorId, string siteId)
        {
            var settings = await GetVendorSettingsAsync(vendorId, siteId);
            
            return new ApiSettings
            {
                Endpoint = settings.ApiEndpoint,
                Key = settings.ApiKey,
                Secret = settings.ApiSecret
            };
        }

        public async Task<BetLimits> GetBetLimitsAsync(string vendorId, string siteId)
        {
            var settings = await GetVendorSettingsAsync(vendorId, siteId);
            
            return new BetLimits
            {
                MinBet = settings.MinBet,
                MaxBet = settings.MaxBet
            };
        }

        public async Task<bool> ValidateVendorAsync(string vendorId, string siteId)
        {
            var settings = await GetVendorSettingsAsync(vendorId, siteId);
            return settings.IsActive;
        }

        private async Task<VendorSettings> GetVendorSettingsAsync(string vendorId, string siteId)
        {
            var settings = await _vendorSettings
                .Find(x => x.VendorId == vendorId && x.SiteId == siteId)
                .FirstOrDefaultAsync();

            if (settings == null)
            {
                throw new InvalidOperationException($"Vendor settings not found for vendor {vendorId} and site {siteId}");
            }

            return settings;
        }
    }

    public class ApiSettings
    {
        public required string Endpoint { get; set; }
        public required string Key { get; set; }
        public required string Secret { get; set; }
    }

    public class BetLimits
    {
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
    }
} 