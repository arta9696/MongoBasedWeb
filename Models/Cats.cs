using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoBasedWeb.Models
{
    public class Cats
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set;} = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public List<string> Friends { get; set; } = new List<string>();
        public Attributes Attribute { get; set; }
    }
}
