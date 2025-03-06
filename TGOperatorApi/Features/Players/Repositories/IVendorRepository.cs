using TgOperator.Features.Players.Models;
using TGOperatorApi.Features.Players.Models;

namespace TgOperator.Features.Players.Repositories
{
    public interface IVendorRepository
    {
        Task<Vendor?> GetByVendorIdAsync(string vendorId, CancellationToken cancellationToken = default);
    }
}