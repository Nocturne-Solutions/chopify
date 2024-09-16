using chopify.Models;
using chopify.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace chopify.Controllers
{
    [ApiController]
    [Route("suggestion")]
    public class SuggestionController(ISuggestionService suggestionService) : Controller
    {
        private readonly ISuggestionService _suggestionService = suggestionService;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SuggestSong(SuggestionUpsertDTO suggestion)
        {
            var result = await _suggestionService.SuggestSong(suggestion);

            switch (result)
            {
                case ISuggestionService.ResultCodes.SongNotFound:
                    return NotFound(new { message = "No se pudo sugerir la canción debido a que no se encontró la canción en Spotify." });
                case ISuggestionService.ResultCodes.SongAlreadySuggested:
                    return Conflict(new { message = "No se pudo sugerir la canción debido a que ya fue sugerida." });
                case ISuggestionService.ResultCodes.UserAlreadySuggested:
                    return Conflict(new { message = "No se pudo sugerir la canción debido a que ya fue sugerida por el usuario." });
                case ISuggestionService.ResultCodes.NoRoundInProgress:
                    return BadRequest(new { message = "No se pudo sugerir la canción debido a que no hay una ronda en curso." });
                case ISuggestionService.ResultCodes.Success:
                    break;
            }

            return Ok(new { message = "Canción sugerida exitosamente." });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll() =>
            Ok(await _suggestionService.GetAllAsync());
    }
}
