using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TgOperator.Features.Players.Models
{
    public enum BetTransactionType
    {
        Bet,
        Settle,
        Void
    }

    public class BetTransaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public required string TransactionId { get; set; }
        public required string PlayerId { get; set; }
        public string? SiteId { get; set; }
        public BetTransactionType Type { get; set; }
        public long GameRoundId { get; set; }
        public int GameTypeId { get; set; }
        public required string Currency { get; set; }

        // Bet specific fields
        public decimal? BetAmount { get; set; }
        public bool IsFreeBet { get; set; }
        public bool IsFreeSpin { get; set; }
        public decimal ContributionAmount { get; set; }

        // Settle specific fields
        public decimal? TotalWin { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? ValidBetAmount { get; set; }
        public DateTime? SettleTime { get; set; }

        public DateTime CreatedAt { get; set; }
    }
} 