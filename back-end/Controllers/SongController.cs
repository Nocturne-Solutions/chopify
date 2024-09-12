using chopify.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace chopify.Controllers
{
    [ApiController]
    [Route("song")]
    public class SongController(ISongService songService) : Controller
    {
        private readonly ISongService _songService = songService;

        [HttpGet("fetch/{search}")]
        [Authorize]
        public async Task<IActionResult> Fetch(string search)
        {
            var song = await _songService.FetchAsync(search);

            if (song == null)
                return NotFound();

            return Ok(song);
        }

        [HttpGet("most-popular")]
        [Authorize]
        public async Task<IActionResult> MostPopularSongsArgentina()
        {
            var songs = await _songService.GetMostPopularSongsArgentinaAsync();

            if (songs == null)
                return NotFound();

            return Ok(songs);
        }
    }
}
