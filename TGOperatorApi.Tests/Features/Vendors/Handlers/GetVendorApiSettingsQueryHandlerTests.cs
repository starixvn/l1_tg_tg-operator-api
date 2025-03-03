using Xunit;
using NSubstitute;
using MongoDB.Driver;
using MediatR;
using TgOperator.Features.Vendors.Models;
using TgOperator.Features.Vendors.Queries;
using TgOperator.Features.Vendors.Handlers;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;

namespace TGOperatorApi.Tests.Features.Vendors.Handlers
{
    public class GetVendorApiSettingsQueryHandlerTests
    {
        private readonly IMongoCollection<VendorSettings> _vendorSettingsCollection;
        private readonly GetVendorApiSettingsQueryHandler _handler;

        public GetVendorApiSettingsQueryHandlerTests()
        {
            var mongoDbFactory = Substitute.For<IMongoDbFactory>();
            _vendorSettingsCollection = Substitute.For<IMongoCollection<VendorSettings>>();
            
            var mongoDbSettings = Substitute.For<IOptions<MongoDbSettings>>();
            mongoDbSettings.Value.Returns(new MongoDbSettings { 
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "tg_operator_test",
                VendorSettingsCollectionName = "vendor_settings",
                BetTransactionsCollectionName = "bet_transactions",
                OperatorApiLogsCollectionName = "operator_api_logs"
            });

            mongoDbFactory.GetCollection<VendorSettings>(Arg.Is<string>(x => x == "vendor_settings"))
                .Returns(_vendorSettingsCollection);
            
            _handler = new GetVendorApiSettingsQueryHandler(mongoDbFactory, mongoDbSettings);
        }

        [Fact]
        public async Task Handle_WhenVendorExists_ReturnsApiSettings()
        {
            // Arrange
            var vendorId = "vendor123";
            var siteId = "site123";
            var vendorSettings = new VendorSettings
            {
                VendorId = vendorId,
                SiteId = siteId,
                ApiEndpoint = "https://api.example.com",
                ApiKey = "test-key",
                ApiSecret = "test-secret",
                Currency = "USD",
                IsActive = true,
                MinBet = 1,
                MaxBet = 1000,
                MaxWin = 10000,
                LastUpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var mockCursor = Substitute.For<IAsyncCursor<VendorSettings>>();
            mockCursor.MoveNextAsync(default).Returns(true, false);
            mockCursor.Current.Returns(new List<VendorSettings> { vendorSettings });

            _vendorSettingsCollection.FindAsync(
                Arg.Any<FilterDefinition<VendorSettings>>(),
                Arg.Any<FindOptions<VendorSettings>>(),
                Arg.Any<CancellationToken>())
                .Returns(mockCursor);

            var query = new GetVendorApiSettingsQuery(vendorId, siteId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vendorSettings.ApiEndpoint, result.Endpoint);
            Assert.Equal(vendorSettings.ApiKey, result.Key);
            Assert.Equal(vendorSettings.ApiSecret, result.Secret);
        }

        [Fact]
        public async Task Handle_WhenVendorNotFound_ThrowsException()
        {
            // Arrange
            var vendorId = "nonexistent";
            var siteId = "site123";

            var mockCursor = Substitute.For<IAsyncCursor<VendorSettings>>();
            mockCursor.MoveNextAsync(default).Returns(false);
            mockCursor.Current.Returns(new List<VendorSettings>());

            _vendorSettingsCollection.FindAsync(
                Arg.Any<FilterDefinition<VendorSettings>>(),
                Arg.Any<FindOptions<VendorSettings>>(),
                Arg.Any<CancellationToken>())
                .Returns(mockCursor);

            var query = new GetVendorApiSettingsQuery(vendorId, siteId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(query, CancellationToken.None));
            
            Assert.Contains("Vendor settings not found", exception.Message);
        }
    }
} 