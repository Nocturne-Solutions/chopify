using MongoDB.Bson;

namespace chopify.Data.Entities
{
    public class Suggestions : IEntity
    {
        public ObjectId Id { get; set; }

        public ObjectId SongMongoId {  get; set; }

        public string SongSpotifyId { get; set; } = string.Empty;

        public ObjectId UserId { get; set; }

        public int Votes {  get; set; }
    }
}
