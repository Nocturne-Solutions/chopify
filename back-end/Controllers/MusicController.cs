using chopify.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace chopify.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MusicController : Controller
    {
        private readonly IMongoDatabase _database;

        public MusicController(IMongoDatabase database)
        {
            _database = database;
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult> Get()
        {
            var collection = _database.GetCollection<Music>("music");

            var musicList = await collection.Find(_ => true)
                                            .Project(m => new
                                            {
                                                id = m.InternalId,
                                                m.Name,
                                                m.Duration
                                            })
                                            .ToListAsync();

            return Ok(musicList);
        }
    }
}
