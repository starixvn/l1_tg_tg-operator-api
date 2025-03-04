using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using TgOperator.Core.Features.Wallet.Balance;

namespace TgOperator.Core.Features.Wallet
{
    public class WalletService : IWalletService
    {
        private readonly HttpClient _httpClient;
        private readonly WalletApiSettings _settings;

        public WalletService(HttpClient httpClient, IOptions<WalletApiSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        public async Task<GetBalanceResponse> GetBalanceAsync(string playerId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Wallet/balance", 
                new { playerId }, 
                cancellationToken);
            
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<GetBalanceResponse>(cancellationToken: cancellationToken);
            return result ?? throw new InvalidOperationException("Failed to deserialize wallet balance response");
        }

        // Future methods will be added here:
        // public async Task<AddBalanceResponse> AddBalanceAsync(...)
        // public async Task<DeductBalanceResponse> DeductBalanceAsync(...)
    }
} 