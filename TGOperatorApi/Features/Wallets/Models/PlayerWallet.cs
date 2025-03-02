using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TgOperator.Features.Wallets.Models
{
    public class PlayerWallet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        public required string PlayerId { get; set; }
        public required string OperatorId { get; set; }
        public required string SiteId { get; set; }
        public decimal Balance { get; set; }
        public required string Currency { get; set; }
        
        // Track last transaction for consistency
        public required string LastTransactionId { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
} 