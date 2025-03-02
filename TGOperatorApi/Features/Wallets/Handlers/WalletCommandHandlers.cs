using MediatR;
using MongoDB.Driver;
using TgOperator.Features.Wallets.Commands;
using TgOperator.Features.Wallets.Models;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using Sentry;

namespace TgOperator.Features.Wallets.Handlers
{
    public class DepositCommandHandler : IRequestHandler<DepositCommand, WalletResponse>
    {
        private readonly IMongoCollection<PlayerWallet> _wallets;
        private readonly IMongoCollection<Transaction> _transactions;
        private readonly IHub _sentryHub;

        public DepositCommandHandler(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings,
            IHub sentryHub)
        {
            _wallets = mongoDbFactory.GetCollection<PlayerWallet>(
                mongoDbSettings.Value.PlayerWalletsCollectionName);
            _transactions = mongoDbFactory.GetCollection<Transaction>(
                mongoDbSettings.Value.TransactionsCollectionName);
            _sentryHub = sentryHub;
        }

        public async Task<WalletResponse> Handle(DepositCommand command, CancellationToken cancellationToken)
        {
            var session = await _wallets.Database.Client.StartSessionAsync();
            try
            {
                session.StartTransaction();

                var request = command.Request;
                var transactionId = Guid.NewGuid().ToString();

                // Find or create wallet
                var wallet = await _wallets.Find(session, x =>
                    x.PlayerId == request.PlayerId &&
                    x.VendorId == request.VendorId &&
                    x.SiteId == request.SiteId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (wallet == null)
                {
                    wallet = new PlayerWallet
                    {
                        PlayerId = request.PlayerId,
                        VendorId = request.VendorId,
                        SiteId = request.SiteId,
                        Balance = 0,
                        Currency = request.Currency,
                        LastTransactionId = transactionId,
                        LastUpdatedAt = DateTime.UtcNow
                    };
                    await _wallets.InsertOneAsync(session, wallet, cancellationToken: cancellationToken);
                }

                // Create transaction record
                var transaction = new Transaction
                {
                    TransactionId = transactionId,
                    PlayerId = request.PlayerId,
                    VendorId = request.VendorId,
                    SiteId = request.SiteId,
                    Type = "deposit",
                    Category = "payment",
                    Amount = request.Amount,
                    Currency = request.Currency,
                    BalanceBefore = wallet.Balance,
                    BalanceAfter = wallet.Balance + request.Amount,
                    Status = "completed",
                    Metadata = new Dictionary<string, object>
                    {
                        { "paymentMethod", request.PaymentMethod },
                        { "paymentReference", request.PaymentReference }
                    },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CompletedAt = DateTime.UtcNow
                };

                await _transactions.InsertOneAsync(session, transaction, cancellationToken: cancellationToken);

                // Update wallet balance
                var update = Builders<PlayerWallet>.Update
                    .Inc(x => x.Balance, request.Amount)
                    .Set(x => x.LastTransactionId, transactionId)
                    .Set(x => x.LastUpdatedAt, DateTime.UtcNow);

                var updateResult = await _wallets.UpdateOneAsync(
                    session,
                    x => x.Id == wallet.Id,
                    update,
                    cancellationToken: cancellationToken);

                if (updateResult.ModifiedCount != 1)
                {
                    throw new InvalidOperationException("Failed to update wallet balance");
                }

                await session.CommitTransactionAsync(cancellationToken);

                return new WalletResponse
                {
                    TransactionId = transactionId,
                    Balance = transaction.BalanceAfter,
                    Currency = request.Currency,
                    Status = "success",
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync(cancellationToken);
                _sentryHub.CaptureException(ex);
                throw;
            }
            finally
            {
                session?.Dispose();
            }
        }
    }
} 