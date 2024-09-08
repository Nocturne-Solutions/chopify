using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace chopify.Data.Entities
{
    public class User : IEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("Tag")]
        public string Tag { get; set; } = string.Empty;
    }
}
