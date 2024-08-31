using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace chopify.Entities
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }  // Cambia a ObjectId para manejo automático por MongoDB

        [BsonElement("username")]
        public string Username { get; set; } = null!;

        [BsonElement("tag")]
        public string Tag { get; set; } = null!;
    }
}
