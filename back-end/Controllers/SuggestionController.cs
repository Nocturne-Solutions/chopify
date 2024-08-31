using chopify.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace chopify.Controllers
{
    [ApiController]
    [Route("suggestion")]
    public class SuggestionController : Controller
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Suggestion> _suggestionCollection;
        private readonly IMongoCollection<Music> _musicCollection;

        public SuggestionController(IMongoDatabase database)
        {
            _database = database;
            _suggestionCollection = _database.GetCollection<Suggestion>("suggestions");
            _musicCollection = _database.GetCollection<Music>("music");
        }

        [HttpPost]
        [Route("suggest")]
        public async Task<ActionResult> Suggest([FromBody] SuggestionRequest request)
        {
            // Verificar si la música existe
            var music = await _musicCollection.Find(m => m.InternalId == request.MusicId).FirstOrDefaultAsync();
            if (music == null)
            {
                return NotFound("Music not found.");
            }

            // Actualizar o insertar la sugerencia
            var filter = Builders<Suggestion>.Filter.Eq(s => s.MusicId, request.MusicId);
            var update = Builders<Suggestion>.Update
                .Set(s => s.MusicName, music.Name)
                .Inc(s => s.Count, 1);
            var options = new FindOneAndUpdateOptions<Suggestion> { IsUpsert = true, ReturnDocument = ReturnDocument.After };

            var result = await _suggestionCollection.FindOneAndUpdateAsync(filter, update, options);

            return Ok(new
            {
                result.MusicId,
                result.MusicName,
                result.Count
            });
        }
    }

    public class SuggestionRequest
    {
        public int MusicId { get; set; }
    }
}
