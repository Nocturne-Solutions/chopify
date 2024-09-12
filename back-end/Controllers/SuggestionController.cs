using chopify.Models;
using chopify.Services.Implementations;
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
            if (await _suggestionService.SuggestSong(suggestion))
                return Ok();
            else
                return Conflict(new { message = "No se pudo sugerir la canción debido a que ya fue sugerida o no es una canción valida." });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll() =>
            Ok(await _suggestionService.GetAllAsync());
    }
}
