using MediatR;
using MongoDB.Driver;
using TgOperator.Features.Players.Models;
using TgOperator.Features.Players.Commands;
using TgOperator.Shared.MongoDB;
using Sentry;

namespace TgOperator.Features.Players.Handlers
{
    public class PlaceBetCommandHandler : IRequestHandler<PlaceBetCommand, PlaceBetResponse>
    {
        private readonly IMongoCollection<BetTransaction> _betTransactions;
        private readonly IHub _sentryHub;

        public PlaceBetCommandHandler(
            IMongoDbFactory mongoDbFactory,
            IHub sentryHub)
        {
            _betTransactions = mongoDbFactory.GetCollection<BetTransaction>();
            _sentryHub = sentryHub;
        }

        public async Task<PlaceBetResponse> Handle(PlaceBetCommand command, CancellationToken cancellationToken)
        {
            //TODO: validate operator token
            //TODO: validate player id

            //TODO: validate bet command
            
            var request = command.Request;
            var now = DateTime.UtcNow;

            // Create bet transaction
            var transaction = new BetTransaction
            {
                TransactionId = request.Seq,
                PlayerId = request.PlayerId,
                SiteId = request.SiteId,
                Type = BetTransactionType.Bet,
                GameRoundId = request.GameRoundId,
                GameTypeId = request.GameTypeId,
                Currency = request.Currency.ToUpper(),
                BetAmount = request.Amount,
                IsFreeBet = request.FreeBet,
                IsFreeSpin = request.FreeSpin,
                ContributionAmount = request.ContributionAmount,
                CreatedAt = request.TimeStamp
            };

            try
            {
                //TODO: deduct balance from wallet
                await _betTransactions.InsertOneAsync(transaction, cancellationToken);

                //TODO: insert operator api log

                return new PlaceBetResponse
                {
                    TimeStamp = now,
                    Result = new PlaceBetResult
                    {
                        Balance = 24250, // TODO: Get actual balance from player's wallet
                        ErrorCode = 0,
                        ErrorDescription = "Success",
                        PlayerId = request.PlayerId,
                        MinBet = 1, // TODO: Get from vendor settings
                        MaxBet = 2000, // TODO: Get from vendor settings
                        ObTransId = long.Parse(request.Seq) // TODO: Verify if this is the correct mapping
                    }
                };
            }
            catch (Exception ex)
            {
                _sentryHub.CaptureException(ex, scope =>
                {
                    scope.SetExtra("placeBetRequest", request);
                });

                return new PlaceBetResponse
                {
                    TimeStamp = now,
                    Result = new PlaceBetResult
                    {   
                        PlayerId = request.PlayerId,
                        Balance = 0.0m,
                        ErrorCode = 3002,
                        ErrorDescription = "Player has insufficient funds.",
                        MinBet = 0,
                        MaxBet = 0,
                        ObTransId = 0
                    }
                };
            }
        }
    }
} 