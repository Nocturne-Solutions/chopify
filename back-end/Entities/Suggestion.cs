using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace chopify.Entities
{
    public class Suggestion
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("musicId")]
        public int MusicId { get; set; }

        [BsonElement("musicName")]
        public string MusicName { get; set; } = null!;

        [BsonElement("count")]
        public int Count { get; set; }
    }
}
