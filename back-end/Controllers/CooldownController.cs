using chopify.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace chopify.Controllers
{
    [ApiController]
    [Route("cooldown")]
    public class CooldownController(ICooldownService cooldownService) : Controller
    {
        private readonly ICooldownService _cooldownService = cooldownService;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll() =>
            Ok(await _cooldownService.GetAllAsync());
    }
}
