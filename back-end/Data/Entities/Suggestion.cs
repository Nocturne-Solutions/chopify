using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace chopify.Data.Entities
{
    public class Suggestion : IEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("SpotifySongId")]
        public string SpotifySongId { get; set; } = string.Empty;

        [BsonElement("Artist")]
        public string Artist { get; set; } = string.Empty;

        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("FirstReleaseDate")]
        public string FirstReleaseDate { get; set; } = string.Empty;

        [BsonElement("Duration")]
        public TimeSpan? Duration { get; set; }

        [BsonElement("CoverUrl")]
        public string CoverUrl { get; set; } = string.Empty;

        [BsonElement("SuggestedBy")]
        public string SuggestedBy { get; set; } = string.Empty;
        
        [BsonElement("Votes")]
        public int Votes {  get; set; }
    }
}
