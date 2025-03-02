using Xunit;
using NSubstitute;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TgOperator.Features.Wallets;
using TgOperator.Features.Wallets.Models;
using TgOperator.Features.Wallets.Queries;
using TgOperator.Features.Wallets.Handlers;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using TgOperator.Shared.MongoDB;
using Sentry;

namespace TGOperatorApi.Tests.Features.Wallets
{
    public class GetBalanceTests
    {
        private readonly IMediator _mediator;
        private readonly IHub _sentryHub;
        private readonly WalletController _controller;
        private readonly IMongoCollection<PlayerWallet> _walletCollection;
        private readonly GetBalanceQueryHandler _handler;

        public GetBalanceTests()
        {
            // Setup for controller tests
            _mediator = Substitute.For<IMediator>();
            _sentryHub = Substitute.For<IHub>();
            _controller = new WalletController(_mediator, _sentryHub);

            // Setup for handler tests
            var mongoDbFactory = Substitute.For<IMongoDbFactory>();
            _walletCollection = Substitute.For<IMongoCollection<PlayerWallet>>();
            var mongoDbSettings = Substitute.For<IOptions<MongoDbSettings>>();
            mongoDbSettings.Value.Returns(new MongoDbSettings { 
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "tg_operator_test",
                PlayerWalletsCollectionName = "player_wallets",
                PlayersCollectionName = "players",
                TransactionsCollectionName = "transactions",
                GameTransactionsCollectionName = "game_transactions"
            });
            mongoDbFactory.GetCollection<PlayerWallet>(Arg.Any<string>()).Returns(_walletCollection);
            
            _handler = new GetBalanceQueryHandler(mongoDbFactory, mongoDbSettings);
        }

        [Fact]
        public async Task GetBalance_WhenWalletExists_ReturnsOkWithBalance()
        {
            // Arrange
            var playerId = "player123";
            var siteId = "site123";
            var expectedResponse = new GetBalanceResponse
            {
                PlayerId = playerId,
                SiteId = siteId,
                Balance = 1000.50m,
                Currency = "USD",
                LastUpdatedAt = DateTime.UtcNow
            };

            _mediator.Send(Arg.Any<GetBalanceQuery>()).Returns(expectedResponse);

            // Act
            var result = await _controller.GetBalance(playerId, siteId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<GetBalanceResponse>(okResult.Value);
            Assert.Equal(expectedResponse.Balance, response.Balance);
            Assert.Equal(expectedResponse.Currency, response.Currency);
            Assert.Equal(expectedResponse.PlayerId, response.PlayerId);
            Assert.Equal(expectedResponse.SiteId, response.SiteId);
        }

        [Fact]
        public async Task GetBalance_WhenWalletNotFound_ReturnsNotFound()
        {
            // Arrange
            var playerId = "nonexistent";
            var siteId = "site123";
            _mediator.Send(Arg.Any<GetBalanceQuery>())
                .Returns(Task.FromException<GetBalanceResponse>(
                    new InvalidOperationException("Wallet not found")));

            // Act
            var result = await _controller.GetBalance(playerId, siteId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetBalanceHandler_WhenWalletExists_ReturnsBalance()
        {
            // Arrange
            var playerId = "player123";
            var siteId = "site123";
            var wallet = new PlayerWallet
            {
                PlayerId = playerId,
                SiteId = siteId,
                VendorId = "vendor123",
                Balance = 1000.50m,
                Currency = "USD",
                LastTransactionId = "tx123",
                LastUpdatedAt = DateTime.UtcNow
            };

            var mockCursor = Substitute.For<IAsyncCursor<PlayerWallet>>();
            mockCursor.MoveNextAsync(default).Returns(true, false);
            mockCursor.Current.Returns(new List<PlayerWallet> { wallet });

            _walletCollection.FindAsync(
                Arg.Any<FilterDefinition<PlayerWallet>>(),
                Arg.Any<FindOptions<PlayerWallet>>(),
                Arg.Any<CancellationToken>())
                .Returns(mockCursor);

            // Act
            var result = await _handler.Handle(new GetBalanceQuery(playerId, siteId), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(wallet.Balance, result.Balance);
            Assert.Equal(wallet.Currency, result.Currency);
            Assert.Equal(wallet.PlayerId, result.PlayerId);
            Assert.Equal(wallet.SiteId, result.SiteId);
        }

        [Fact]
        public async Task GetBalanceHandler_WhenWalletNotFound_ThrowsException()
        {
            // Arrange
            var playerId = "nonexistent";
            var siteId = "site123";

            var mockCursor = Substitute.For<IAsyncCursor<PlayerWallet>>();
            mockCursor.MoveNextAsync(default).Returns(false);
            mockCursor.Current.Returns(new List<PlayerWallet>());

            _walletCollection.FindAsync(
                Arg.Any<FilterDefinition<PlayerWallet>>(),
                Arg.Any<FindOptions<PlayerWallet>>(),
                Arg.Any<CancellationToken>())
                .Returns(mockCursor);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(new GetBalanceQuery(playerId, siteId), CancellationToken.None));
            
            Assert.Contains("Wallet not found", exception.Message);
        }
    }
} 