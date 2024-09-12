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
            var result = await _voteService.AddVoteAsync(vote);

            switch (result)
            {
                case IVoteService.ResultCode.UserAlreadyVoted:
                    return Conflict(new { message = "No se pudo emitir el voto debido a que el usuario ya emitio un voto." });
                case IVoteService.ResultCode.SongNotSuggested:
                    return Conflict(new { message = "No se pudo emitir el voto debido a que la canción que se quiere votar no fue sugerida." });
                case IVoteService.ResultCode.Success:
                    break;
            }

            return Ok(new { message = "Voto emitido exitosamente." });
        }

        [HttpGet("{user}")]
        [Authorize]
        public async Task<IActionResult> Get(string user)
        {
            var vote = await _voteService.GetByUserAsync(user);

            if (vote == null)
                return NotFound();

            return Ok(vote);
        }
    }
}
