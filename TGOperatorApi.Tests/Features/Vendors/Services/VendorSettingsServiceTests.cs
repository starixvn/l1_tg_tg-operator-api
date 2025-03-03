using Xunit;
using NSubstitute;
using MongoDB.Driver;
using TgOperator.Features.Vendors.Models;
using TgOperator.Features.Vendors.Services;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;

namespace TGOperatorApi.Tests.Features.Vendors.Services
{
    public class VendorSettingsServiceTests
    {
        private readonly IMongoCollection<VendorSettings> _vendorSettingsCollection;
        private readonly VendorSettingsService _service;

        public VendorSettingsServiceTests()
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
            
            _service = new VendorSettingsService(mongoDbFactory, mongoDbSettings);
        }

        [Fact]
        public async Task GetApiSettings_WhenVendorExists_ReturnsApiSettings()
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

            // Act
            var result = await _service.GetApiSettingsAsync(vendorId, siteId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vendorSettings.ApiEndpoint, result.Endpoint);
            Assert.Equal(vendorSettings.ApiKey, result.Key);
            Assert.Equal(vendorSettings.ApiSecret, result.Secret);
        }

        [Fact]
        public async Task GetBetLimits_WhenVendorExists_ReturnsBetLimits()
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

            // Act
            var result = await _service.GetBetLimitsAsync(vendorId, siteId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vendorSettings.MinBet, result.MinBet);
            Assert.Equal(vendorSettings.MaxBet, result.MaxBet);
        }

        [Fact]
        public async Task ValidateVendor_WhenVendorExistsAndActive_ReturnsTrue()
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

            // Act
            var result = await _service.ValidateVendorAsync(vendorId, siteId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateVendor_WhenVendorExistsAndInactive_ReturnsFalse()
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
                IsActive = false,
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

            // Act
            var result = await _service.ValidateVendorAsync(vendorId, siteId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateVendor_WhenVendorNotFound_ThrowsException()
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

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.ValidateVendorAsync(vendorId, siteId));
            
            Assert.Contains("Vendor settings not found", exception.Message);
        }
    }
} 