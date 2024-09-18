using chopify.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace chopify.Controllers
{
    [ApiController]
    [Route("voting-system")]
    public class VotingSystemController(IVotingSystemService votingRoundService) : Controller
    {
        private readonly IVotingSystemService _votingRoundService = votingRoundService;

        [HttpPost("start")]
        public async Task<IActionResult> StartNewRound()
        {
            var result = await _votingRoundService.Start();

            switch (result)
            {
                case IVotingSystemService.ResultCode.IsActive:
                    return BadRequest(new { message = "El sistema ya esta en marcha." });
                case IVotingSystemService.ResultCode.FailToGetFirstSong:
                    return BadRequest(new { message = "No se pudo obtener la primera canción." });
                case IVotingSystemService.ResultCode.Success:
                case IVotingSystemService.ResultCode.IsNotActive:
                    break;
            }

            return Ok(new { message = "Sistema de votación iniciado." });
        }

        [HttpPost("stop")]
        public async Task<IActionResult> Stop()
        {
            var result = await _votingRoundService.Stop();

            switch (result)
            {
                case IVotingSystemService.ResultCode.IsNotActive:
                    return BadRequest(new { message = "El sistema no esta en marcha." });
                case IVotingSystemService.ResultCode.Success:
                case IVotingSystemService.ResultCode.IsActive:
                    break;
            }

            return Ok(new { message = "Sistema de votación detenido." });
        }

        [HttpPost("reset")]
        public async Task<IActionResult> Reset()
        {
            var result = await _votingRoundService.Reset();

            switch (result)
            {
                case IVotingSystemService.ResultCode.IsNotActive:
                    return BadRequest(new { message = "El sistema no esta en marcha." });
                case IVotingSystemService.ResultCode.Success:
                case IVotingSystemService.ResultCode.IsActive:
                    break;
            }

            return Ok(new { message = "Sistema de votación reiniciado." });
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetRoundStatus() =>
            Ok(await _votingRoundService.GetStatus());
    }
}
