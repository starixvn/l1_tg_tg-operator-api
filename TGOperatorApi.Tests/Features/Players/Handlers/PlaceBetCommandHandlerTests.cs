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
    public class PlaceBetCommandHandlerTests
    {
        private readonly IMongoCollection<BetTransaction> _betTransactionsCollection;
        private readonly IHub _sentryHub;
        private readonly PlaceBetCommandHandler _handler;

        public PlaceBetCommandHandlerTests()
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
            
            _handler = new PlaceBetCommandHandler(mongoDbFactory, mongoDbSettings, _sentryHub);
        }

        [Fact]
        public async Task Handle_WhenValidRequest_CreatesBetTransaction()
        {
            // Arrange
            var request = new PlaceBetRequest
            {
                TimeStamp = DateTime.UtcNow,
                Seq = "1763178465",
                SiteId = "181",
                GameRoundId = 7222,
                GameTypeId = 1,
                ChoiceId = 1,
                Amount = 225.25m,
                Currency = "usd",
                PlayerId = "167834",
                Ip = "127.0.0.1",
                RoundEndTime = DateTime.UtcNow.AddMinutes(19),
                FreeSpin = false,
                FreeBet = false,
                ContributionRate = 0.025m,
                ContributionAmount = 2.25m,
                OperatorToken = "5d7aa6fb-a90c-482b-bc2e-12b03faf8ee4"
            };

            var command = new PlaceBetCommand(request);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(0, result.Result.ErrorCode);
            Assert.Equal("Success", result.Result.ErrorDescription);
            Assert.Equal(request.PlayerId, result.Result.PlayerId);
            Assert.Equal(1, result.Result.MinBet);
            Assert.Equal(2000, result.Result.MaxBet);
            Assert.Equal(1763178465, result.Result.ObTransId);

            await _betTransactionsCollection.Received(1).InsertOneAsync(
                Arg.Is<BetTransaction>(t => 
                    t.TransactionId == request.Seq &&
                    t.PlayerId == request.PlayerId &&
                    t.BetAmount == request.Amount &&
                    t.Type == BetTransactionType.Bet &&
                    t.GameTypeId == request.GameTypeId &&
                    t.IsFreeBet == request.FreeBet &&
                    t.IsFreeSpin == request.FreeSpin &&
                    t.ContributionAmount == request.ContributionAmount),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenFreeBet_CreatesBetTransactionWithFreeBetFlag()
        {
            // Arrange
            var request = new PlaceBetRequest
            {
                TimeStamp = DateTime.UtcNow,
                Seq = "1763178465",
                SiteId = "181",
                GameRoundId = 7222,
                GameTypeId = 1,
                ChoiceId = 1,
                Amount = 225.25m,
                Currency = "usd",
                PlayerId = "167834",
                Ip = "127.0.0.1",
                RoundEndTime = DateTime.UtcNow.AddMinutes(19),
                FreeSpin = false,
                FreeBet = true,
                ContributionRate = 0.025m,
                ContributionAmount = 2.25m,
                OperatorToken = "5d7aa6fb-a90c-482b-bc2e-12b03faf8ee4"
            };

            var command = new PlaceBetCommand(request);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(0, result.Result.ErrorCode);
            Assert.Equal("Success", result.Result.ErrorDescription);

            await _betTransactionsCollection.Received(1).InsertOneAsync(
                Arg.Is<BetTransaction>(t => 
                    t.IsFreeBet == true &&
                    t.IsFreeSpin == false),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenError_ReturnsFailureResponse()
        {
            // Arrange
            var request = new PlaceBetRequest
            {
                TimeStamp = DateTime.UtcNow,
                Seq = "1763178465",
                SiteId = "181",
                GameRoundId = 7222,
                GameTypeId = 1,
                ChoiceId = 1,
                Amount = 225.25m,
                Currency = "usd",
                PlayerId = "167834",
                Ip = "127.0.0.1",
                RoundEndTime = DateTime.UtcNow.AddMinutes(19),
                FreeSpin = false,
                FreeBet = false,
                ContributionRate = 0.025m,
                ContributionAmount = 2.25m,
                OperatorToken = "5d7aa6fb-a90c-482b-bc2e-12b03faf8ee4"
            };

            var command = new PlaceBetCommand(request);
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
            Assert.Equal(0.0m, result.Result.Balance);
            Assert.Equal(3002, result.Result.ErrorCode);
            Assert.Equal("Player has insufficient funds.", result.Result.ErrorDescription);
            Assert.Equal(0, result.Result.MinBet);
            Assert.Equal(0, result.Result.MaxBet);
            Assert.Equal(0, result.Result.ObTransId);

            _sentryHub.Received(1).CaptureException(
                Arg.Is<Exception>(e => e.Message == "Database error"),
                Arg.Is<Action<SentryScope>>(scope =>
                {
                    scope(Arg.Any<SentryScope>());
                    return true;
                }));
        }

        [Fact]
        public async Task Handle_WithRealisticData_CreatesCorrectBetTransaction()
        {
            // Arrange
            var timestamp = new DateTime(2024, 3, 19, 15, 31, 0, 10, DateTimeKind.Utc);
            var request = new PlaceBetRequest
            {
                TimeStamp = timestamp,
                Seq = "1763178465",
                SiteId = "181",
                GameRoundId = 7222,
                GameTypeId = 1,
                ChoiceId = 1,
                Amount = 225.25m,
                Currency = "USD",
                PlayerId = "167834",
                Ip = "127.0.0.1",
                RoundEndTime = timestamp.AddMinutes(19),
                FreeSpin = false,
                FreeBet = false,
                ContributionRate = 0.025m,
                ContributionAmount = 2.25m,
                OperatorToken = "5d7aa6fb-a90c-482b-bc2e-12b03faf8ee4"
            };

            var command = new PlaceBetCommand(request);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(0, result.Result.ErrorCode);
            Assert.Equal("Success", result.Result.ErrorDescription);
            Assert.Equal(request.PlayerId, result.Result.PlayerId);
            Assert.Equal(1, result.Result.MinBet);
            Assert.Equal(2000, result.Result.MaxBet);
            Assert.Equal(1763178465, result.Result.ObTransId);

            await _betTransactionsCollection.Received(1).InsertOneAsync(
                Arg.Is<BetTransaction>(t => 
                    t.TransactionId == "1763178465" &&
                    t.PlayerId == "167834" &&
                    t.SiteId == "181" &&
                    t.Type == BetTransactionType.Bet &&
                    t.GameRoundId == 7222 &&
                    t.GameTypeId == 1 &&
                    t.Currency == "USD" &&
                    t.BetAmount == 225.25m &&
                    t.IsFreeBet == false &&
                    t.IsFreeSpin == false &&
                    t.ContributionAmount == 2.25m &&
                    t.CreatedAt == timestamp),
                Arg.Any<CancellationToken>());
        }
    }
} 