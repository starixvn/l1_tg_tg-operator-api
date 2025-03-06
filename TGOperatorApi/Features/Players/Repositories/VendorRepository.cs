using MongoDB.Driver;
using TgOperator.Features.Players.Models;
using TgOperator.Shared.MongoDB;
using TGOperatorApi.Features.Players.Models;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;

namespace TgOperator.Features.Players.Repositories
{
    public class VendorRepository : IVendorRepository
    {
        private readonly IMongoCollection<Vendor> _vendors;

        public VendorRepository(IMongoDbFactory mongoDbFactory, IOptions<MongoDbSettings> settings)
        {
            _vendors = mongoDbFactory.GetCollection<Vendor>(settings.Value.VendorSettingsCollectionName);
        }

        public async Task<Vendor?> GetByVendorIdAsync(string vendorId, CancellationToken cancellationToken = default)
        {
            return await _vendors
                .Find(x => x.VendorId == vendorId)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
} 