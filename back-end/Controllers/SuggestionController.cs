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

        [HttpPost("suggest")]
        [Authorize]
        public async Task<IActionResult> SuggestSong(SuggestionDTO suggestion)
        {
            if (await _suggestionService.SuggestSong(suggestion))
                return Ok();
            else
                return Conflict(new{message = "La canción ya ha sido sugerida."});
        }
    }
}
