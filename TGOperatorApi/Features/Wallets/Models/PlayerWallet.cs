using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TgOperator.Features.Wallets.Models
{
    public class PlayerWallet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string PlayerId { get; set; }
        public string OperatorId { get; set; }
        public string SiteId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        
        // Track last transaction for consistency
        public string LastTransactionId { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
} 