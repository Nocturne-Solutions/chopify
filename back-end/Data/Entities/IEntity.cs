using MongoDB.Bson;

namespace chopify.Data.Entities
{
    public interface IEntity
    {
        public ObjectId Id { get; set; }
    }
}
