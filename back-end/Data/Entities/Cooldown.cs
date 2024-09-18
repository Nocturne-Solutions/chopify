using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace chopify.Data.Entities
{
    public class Cooldown : IEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("SpotifySongId")]
        public string SpotifySongId { get; set; } = string.Empty;

        [BsonElement("CooldownEnd")]
        public DateTime CooldownEnd { get; set; }
    }
}
