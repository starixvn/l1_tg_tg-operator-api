using System.Text.Json.Serialization;

namespace TgOperator.Core.Features.Wallet.Balance
{
    public class GetBalanceResponse
    {
        [JsonPropertyName("statusCode")]
        public string StatusCode { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }
    }
} 