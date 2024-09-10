using chopify.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace chopify.Controllers
{
    [ApiController]
    [Route("music")]
    public class MusicController(IMusicService musicService) : Controller
    {
        private readonly IMusicService _musicService = musicService;

        [HttpGet("fetch/{search}")]
        [Authorize]
        public async Task<IActionResult> Fetch(string search)
        {
            var music = await _musicService.FetchAsync(search);

            if (music == null)
                return NotFound();

            return Ok(music);
        }

        [HttpGet("most-popular")]
        [Authorize]
        public async Task<IActionResult> MostPopularSongsArgentina()
        {
            var music = await _musicService.GetMostPopularSongsArgentinaAsync();

            if (music == null)
                return NotFound();

            return Ok(music);
        }
    }
}
