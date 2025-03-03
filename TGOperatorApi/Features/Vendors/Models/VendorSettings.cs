using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TgOperator.Features.Vendors.Models
{
    public class VendorSettings
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public required string VendorId { get; set; }
        public required string SiteId { get; set; }
        public required string Currency { get; set; }
        public required string ApiEndpoint { get; set; }
        public required string ApiKey { get; set; }
        public required string ApiSecret { get; set; }
        public bool IsActive { get; set; }
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
        public decimal MaxWin { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        [BsonIgnoreIfNull]
        public Dictionary<string, object>? AdditionalSettings { get; set; }
    }
} 