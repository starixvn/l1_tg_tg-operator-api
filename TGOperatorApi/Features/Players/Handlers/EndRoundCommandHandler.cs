using MediatR;
using MongoDB.Driver;
using TgOperator.Features.Players.Models;
using TgOperator.Features.Players.Commands;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using Sentry;

namespace TgOperator.Features.Players.Handlers
{
    public class EndRoundCommandHandler : IRequestHandler<EndRoundCommand, EndRoundResponse>
    {
        private readonly IMongoCollection<BetTransaction> _betTransactions;
        private readonly IHub _sentryHub;

        public EndRoundCommandHandler(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings,
            IHub sentryHub)
        {
            _betTransactions = mongoDbFactory.GetCollection<BetTransaction>(
                mongoDbSettings.Value.BetTransactionsCollectionName);
            _sentryHub = sentryHub;
        }

        public async Task<EndRoundResponse> Handle(EndRoundCommand command, CancellationToken cancellationToken)
        {
            //TODO: validate bet transaction
            //TODO: validate end round command

            var request = command.Request;
            var now = DateTime.UtcNow;

            try
            {
                // Create end round transaction
                var transaction = new BetTransaction
                {
                    TransactionId = request.Seq,
                    PlayerId = request.PlayerId,                    
                    GameRoundId = request.GameRoundId,
                    GameTypeId = request.GameTypeId,
                    Type = BetTransactionType.Settle,
                    Currency = "USD", // TODO: Get from bet transaction
                    BetAmount = request.ValidBetAmount,
                    TotalWin = request.TotalWin,
                    TotalAmount = request.TotalAmount,
                    ValidBetAmount = request.ValidBetAmount,
                    CreatedAt = request.TimeStamp
                };

                //TODO: update player balance
                await _betTransactions.InsertOneAsync(transaction, cancellationToken);

                //TODO: insert operator api log

                return new EndRoundResponse
                {
                    TimeStamp = now,
                    Result = new EndRoundResult
                    {
                        Seq = request.Seq,
                        PlayerId = request.PlayerId,
                        ErrorCode = 0,
                        ErrorDescription = "Success"
                    }
                };
            }
            catch (Exception ex)
            {
                _sentryHub.CaptureException(ex, scope =>
                {
                    scope.SetExtra("endRoundRequest", request);
                });

                return new EndRoundResponse
                {
                    TimeStamp = now,
                    Result = new EndRoundResult
                    {
                        Seq = request.Seq,
                        PlayerId = request.PlayerId,
                        ErrorCode = 1000,
                        ErrorDescription = "Unspecified Error."
                    }
                };
            }
        }
    }
} 