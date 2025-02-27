using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Player
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public string PlayerId { get; set; }
    public string OperatorId { get; set; }
    public string SiteId { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; }
} 