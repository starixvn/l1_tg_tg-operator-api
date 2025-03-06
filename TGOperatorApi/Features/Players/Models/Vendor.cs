using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TGOperatorApi.Features.Players.Models
{
    public class Vendor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string VendorId { get; set; }

        public int MinBet { get; set; }
        public int MaxBet { get; set; }
        public string Name { get; set; }
    }
}
