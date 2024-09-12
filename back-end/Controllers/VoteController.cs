using chopify.Models;
using chopify.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace chopify.Controllers
{
    [ApiController]
    [Route("vote")]
    public class VoteController(IVoteService voteService) : Controller
    {
        private readonly IVoteService _voteService = voteService;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Vote(VoteUpsertDTO vote)
        {
            if (await _voteService.AddVoteAsync(vote))
                return Ok();
            else
                return Conflict(new { message = "No se pudo emitir el voto debido a que el usuario ya emitio un voto o la canción que se quiere votar no fue sugerida." });
        }

        [HttpGet("{user}")]
        [Authorize]
        public async Task<IActionResult> Get(string user) =>
            Ok(await _voteService.GetByUserAsync(user));
    }
}
