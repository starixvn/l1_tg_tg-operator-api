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
    public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, WalletResponse>
    {
        private readonly IMongoCollection<PlayerWallet> _wallets;
        private readonly IMongoCollection<Transaction> _transactions;
        private readonly IHub _sentryHub;

        public WithdrawCommandHandler(
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

        public async Task<WalletResponse> Handle(WithdrawCommand command, CancellationToken cancellationToken)
        {
            var session = await _wallets.Database.Client.StartSessionAsync();
            try
            {
                session.StartTransaction();

                var request = command.Request;
                var transactionId = Guid.NewGuid().ToString();

                // Find wallet and check balance
                var wallet = await _wallets.Find(session, x =>
                    x.PlayerId == request.PlayerId &&
                    x.OperatorId == request.OperatorId &&
                    x.SiteId == request.SiteId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (wallet == null)
                {
                    throw new InvalidOperationException("Wallet not found");
                }

                // Validate withdrawal amount
                if (wallet.Balance < request.Amount)
                {
                    throw new InvalidOperationException("Insufficient balance");
                }

                if (request.Currency != wallet.Currency)
                {
                    throw new InvalidOperationException("Currency mismatch");
                }

                // Create pending withdrawal transaction
                var transaction = new Transaction
                {
                    TransactionId = transactionId,
                    PlayerId = request.PlayerId,
                    OperatorId = request.OperatorId,
                    SiteId = request.SiteId,
                    Type = "withdraw",
                    Category = "payment",
                    Amount = request.Amount,
                    Currency = request.Currency,
                    BalanceBefore = wallet.Balance,
                    BalanceAfter = wallet.Balance - request.Amount,
                    Status = "pending", // Withdrawal starts as pending
                    Metadata = new Dictionary<string, object>
                    {
                        { "paymentMethod", request.PaymentMethod },
                        { "paymentReference", request.PaymentReference }
                    },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _transactions.InsertOneAsync(session, transaction, cancellationToken: cancellationToken);

                // Update wallet balance and hold the withdrawal amount
                var update = Builders<PlayerWallet>.Update
                    .Inc(x => x.Balance, -request.Amount)
                    .Set(x => x.LastTransactionId, transactionId)
                    .Set(x => x.LastUpdatedAt, DateTime.UtcNow);

                var updateResult = await _wallets.UpdateOneAsync(
                    session,
                    x => x.Id == wallet.Id && x.Balance >= request.Amount,
                    update,
                    cancellationToken: cancellationToken);

                if (updateResult.ModifiedCount != 1)
                {
                    throw new InvalidOperationException("Failed to update wallet balance");
                }

                // Complete the transaction
                var transactionUpdate = Builders<Transaction>.Update
                    .Set(x => x.Status, "completed")
                    .Set(x => x.CompletedAt, DateTime.UtcNow)
                    .Set(x => x.UpdatedAt, DateTime.UtcNow);

                await _transactions.UpdateOneAsync(
                    session,
                    x => x.TransactionId == transactionId,
                    transactionUpdate,
                    cancellationToken: cancellationToken);

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
                _sentryHub.CaptureException(ex, scope =>
                {
                    scope.SetExtra("withdrawRequest", command.Request);
                });
                throw;
            }
            finally
            {
                session?.Dispose();
            }
        }
    }
} 