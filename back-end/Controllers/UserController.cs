using chopify.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace chopify.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IMongoDatabase _database;

        public UserController(IMongoDatabase database)
        {
            _database = database;
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create([FromBody] UsernameRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest("Username is required.");
            }

            var user = new User
            {
                Username = request.Username,
                Tag = GenerateShortTag()  // Generar el UUID truncado para el campo Tag
            };

            var collection = _database.GetCollection<User>("users");

            try
            {
                await collection.InsertOneAsync(user);
                return CreatedAtAction(nameof(Create), new {user.Username, user.Tag});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string GenerateShortTag()
        {
            // Generar un UUID y truncarlo a 8 caracteres
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }
    }

    public class UsernameRequest
    {
        public string Username { get; set; } = null!;
    }
}
