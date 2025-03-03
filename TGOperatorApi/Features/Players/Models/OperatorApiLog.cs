using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TgOperator.Features.Players.Models
{
    public class OperatorApiLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public required string Type { get; set; }
        public required object Call { get; set; }
        public required object Response { get; set; }
        public required DateTime Timestamp { get; set; }
    }
} 