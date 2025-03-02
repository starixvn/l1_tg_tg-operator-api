using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace TgOperator.Features.Wallets.Models
{
    public class Transaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public required string TransactionId { get; set; }
        public required string PlayerId { get; set; }
        public required string VendorId { get; set; }
        public required string SiteId { get; set; }
        
        // Transaction type (deposit, withdraw, bet, win, etc.)
        public required string Type { get; set; }
        
        // Transaction category (payment, game, bonus, etc.)
        public required string Category { get; set; }
        
        public decimal Amount { get; set; }
        public required string Currency { get; set; }
        
        // Balance tracking
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        
        // Transaction status (pending, completed, failed, etc.)
        public required string Status { get; set; }
        
        // Additional transaction data
        [BsonIgnoreIfNull]
        public Dictionary<string, object>? Metadata { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        [BsonIgnoreIfNull]
        public DateTime? CompletedAt { get; set; }
    }
} 