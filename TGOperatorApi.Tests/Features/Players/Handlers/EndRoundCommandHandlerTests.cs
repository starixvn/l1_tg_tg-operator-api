using Xunit;
using NSubstitute;
using MongoDB.Driver;
using MediatR;
using TgOperator.Features.Players.Models;
using TgOperator.Features.Players.Commands;
using TgOperator.Features.Players.Handlers;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using Sentry;

namespace TGOperatorApi.Tests.Features.Players.Handlers
{
    public class EndRoundCommandHandlerTests
    {
        private readonly IMongoCollection<BetTransaction> _betTransactionsCollection;
        private readonly IHub _sentryHub;
        private readonly EndRoundCommandHandler _handler;

        public EndRoundCommandHandlerTests()
        {
            var mongoDbFactory = Substitute.For<IMongoDbFactory>();
            _betTransactionsCollection = Substitute.For<IMongoCollection<BetTransaction>>();
            _sentryHub = Substitute.For<IHub>();
            
            var mongoDbSettings = Substitute.For<IOptions<MongoDbSettings>>();
            mongoDbSettings.Value.Returns(new MongoDbSettings { 
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "tg_operator_test",
                VendorSettingsCollectionName = "vendor_settings",
                BetTransactionsCollectionName = "bet_transactions",
                OperatorApiLogsCollectionName = "operator_api_logs"
            });

            mongoDbFactory.GetCollection<BetTransaction>(Arg.Is<string>(x => x == "bet_transactions"))
                .Returns(_betTransactionsCollection);
            
            _handler = new EndRoundCommandHandler(mongoDbFactory, mongoDbSettings, _sentryHub);
        }

        [Fact]
        public async Task Handle_WhenValidRequest_CreatesEndRoundTransaction()
        {
            // Arrange
            var timestamp = new DateTime(2024, 3, 19, 15, 31, 0, 0, DateTimeKind.Utc);
            var request = new EndRoundRequest
            {
                TimeStamp = timestamp,
                Seq = "633b0a18-3cd8-4e59-8c1b-16fc301a5f53",
                GameRoundId = 7222,
                GameTypeId = 1,
                PlayerId = "167834",
                TotalAmount = 200m,
                TotalWin = 200m,
                ValidBetAmount = 200m,
                EndTime = timestamp
            };

            var command = new EndRoundCommand(request);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(request.Seq, result.Result.Seq);
            Assert.Equal(request.PlayerId, result.Result.PlayerId);
            Assert.Equal(0, result.Result.ErrorCode);
            Assert.Equal("Success", result.Result.ErrorDescription);

            await _betTransactionsCollection.Received(1).InsertOneAsync(
                Arg.Is<BetTransaction>(t => 
                    t.TransactionId == request.Seq &&
                    t.PlayerId == request.PlayerId &&
                    t.GameRoundId == request.GameRoundId &&
                    t.GameTypeId == request.GameTypeId &&
                    t.Type == BetTransactionType.Settle &&
                    t.Currency == "USD" &&
                    t.BetAmount == request.ValidBetAmount &&
                    t.TotalWin == request.TotalWin &&
                    t.TotalAmount == request.TotalAmount &&
                    t.ValidBetAmount == request.ValidBetAmount &&
                    t.CreatedAt == request.TimeStamp &&
                    t.CompletedAt == request.EndTime),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenError_ReturnsFailureResponse()
        {
            // Arrange
            var timestamp = new DateTime(2024, 3, 19, 15, 31, 0, 0, DateTimeKind.Utc);
            var request = new EndRoundRequest
            {
                TimeStamp = timestamp,
                Seq = "633b0a18-3cd8-4e59-8c1b-16fc301a5f53",
                GameRoundId = 7222,
                GameTypeId = 1,
                PlayerId = "167834",
                TotalAmount = 200m,
                TotalWin = 200m,
                ValidBetAmount = 200m,
                EndTime = timestamp
            };

            var command = new EndRoundCommand(request);
            var exception = new Exception("Database error");

            _betTransactionsCollection.InsertOneAsync(
                Arg.Any<BetTransaction>(),
                Arg.Any<CancellationToken>())
                .Returns(Task.FromException(exception));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(request.Seq, result.Result.Seq);
            Assert.Equal(1000, result.Result.ErrorCode);
            Assert.Equal("Unspecified Error.", result.Result.ErrorDescription);

            _sentryHub.Received(1).CaptureException(
                Arg.Is<Exception>(e => e.Message == "Database error"),
                Arg.Is<Action<SentryScope>>(scope =>
                {
                    scope(Arg.Any<SentryScope>());
                    return true;
                }));
        }
    }
} 