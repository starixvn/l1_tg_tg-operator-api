using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TgOperator.Features.Players.Models
{
    public enum GameTransactionType
    {
        Bet,
        Settle
    }

    public class GameTransaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string TransactionId { get; set; }
        public string PlayerId { get; set; }
        public string SiteId { get; set; }
        public GameTransactionType Type { get; set; }
        public string Category { get; set; } = "game";
        public long GameRoundId { get; set; }
        public int GameTypeId { get; set; }
        public string Currency { get; set; }

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