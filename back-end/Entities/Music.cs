using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace chopify.Entities
{
    public class Music
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("id")]
        public int InternalId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("duration")]
        public int Duration { get; set; }
    }
}
