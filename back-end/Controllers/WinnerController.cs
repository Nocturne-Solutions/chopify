using chopify.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace chopify.Controllers
{
    [ApiController]
    [Route("winner")]
    public class WinnerController(IWinnerService winnerService) : Controller
    {
        private readonly IWinnerService _winnerService = winnerService;

        [HttpGet("last")]
        [Authorize]
        public async Task<IActionResult> GetLast() =>
            Ok(await _winnerService.GetLastAsync());
    }
}
