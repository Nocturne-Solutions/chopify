using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace chopify.Data.Entities
{
    public class Vote : IEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("SpotifySongId")]
        public string SpotifySongId { get; set; } = string.Empty;

        [BsonElement("User")]
        public string User { get; set; } = string.Empty;
    }
}
