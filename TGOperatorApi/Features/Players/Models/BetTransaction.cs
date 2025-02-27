using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TgOperator.Features.Players.Models
{
    public class BetTransaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string TransactionId { get; set; }
        public long PlayerId { get; set; }
        public string OperatorId { get; set; }
        public string SiteId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public long GameRoundId { get; set; }
        public int GameTypeId { get; set; }
        public string Seq { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFreeBet { get; set; }
        public bool IsFreeSpin { get; set; }
        public decimal ContributionAmount { get; set; }
        public string Status { get; set; }
    }
} 