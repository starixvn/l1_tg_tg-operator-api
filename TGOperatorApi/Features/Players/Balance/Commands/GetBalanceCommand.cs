using System.Text.Json.Serialization;
using MediatR;

namespace TgOperator.Features.Players.Balance.Commands
{
    public class GetBalanceCommand : IRequest<GetBalanceCommand.Response>
    {
        [JsonPropertyName("TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [JsonPropertyName("Seq")]
        public string Seq { get; set; } = string.Empty;

        [JsonPropertyName("SiteId")]
        public string SiteId { get; set; } = string.Empty;

        [JsonPropertyName("PlayerId")]
        public string PlayerId { get; set; } = string.Empty;

        [JsonPropertyName("OperatorToken")]
        public string OperatorToken { get; set; } = string.Empty;

        public class Response
        {
            [JsonPropertyName("TimeStamp")]
            public DateTime TimeStamp { get; set; }

            [JsonPropertyName("Result")]
            public Result Result { get; set; } = null!;
        }

        public class Result
        {
            [JsonPropertyName("Seq")]
            public string Seq { get; set; } = string.Empty;

            [JsonPropertyName("ErrorCode")]
            public int ErrorCode { get; set; }

            [JsonPropertyName("ErrorDescription")]
            public string ErrorDescription { get; set; } = string.Empty;

            [JsonPropertyName("Balance")]
            public decimal Balance { get; set; }
        }
    }
} 