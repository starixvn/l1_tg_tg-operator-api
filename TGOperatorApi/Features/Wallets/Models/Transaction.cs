using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace TgOperator.Features.Wallets.Models
{
    public class Transaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string TransactionId { get; set; }
        public string PlayerId { get; set; }
        public string OperatorId { get; set; }
        public string SiteId { get; set; }
        
        // Transaction type (deposit, withdraw, bet, win, etc.)
        public string Type { get; set; }
        
        // Transaction category (payment, game, bonus, etc.)
        public string Category { get; set; }
        
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        
        // Balance tracking
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        
        // Transaction status (pending, completed, failed, etc.)
        public string Status { get; set; }
        
        // Additional transaction data
        [BsonIgnoreIfNull]
        public Dictionary<string, object> Metadata { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        [BsonIgnoreIfNull]
        public DateTime? CompletedAt { get; set; }
    }
} 