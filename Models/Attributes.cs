using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MongoBasedWeb.Models
{
    public class Attributes
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; }
        public int Cuteness { get; set; }
    }
}
