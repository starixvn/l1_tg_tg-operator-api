using MediatR;
using MongoDB.Driver;
using TgOperator.Features.Wallets.Models;
using TgOperator.Features.Wallets.Queries;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;

namespace TgOperator.Features.Wallets.Handlers
{
    public class GetBalanceQueryHandler : IRequestHandler<GetBalanceQuery, GetBalanceResponse>
    {
        private readonly IMongoCollection<PlayerWallet> _wallets;

        public GetBalanceQueryHandler(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings)
        {
            _wallets = mongoDbFactory.GetCollection<PlayerWallet>(
                mongoDbSettings.Value.PlayerWalletsCollectionName);
        }

        public async Task<GetBalanceResponse> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
        {
            var wallet = await _wallets
                .Find(x => x.PlayerId == request.PlayerId && x.SiteId == request.SiteId)
                .FirstOrDefaultAsync(cancellationToken);

            if (wallet == null)
            {
                throw new InvalidOperationException($"Wallet not found for player {request.PlayerId}");
            }

            return new GetBalanceResponse
            {
                PlayerId = wallet.PlayerId,
                SiteId = wallet.SiteId,
                Balance = wallet.Balance,
                Currency = wallet.Currency,
                LastUpdatedAt = wallet.LastUpdatedAt
            };
        }
    }
} 