using chopify.Models;
using chopify.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace chopify.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController(IUserService userService) : Controller
    {
        private readonly IUserService _userService = userService;

        [HttpPost]
        public async Task<IActionResult> Create(UserUpsertDTO model)
        {
            var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            if (string.IsNullOrWhiteSpace(jwtSecretKey))
                throw new ArgumentNullException("JWT_SECRET_KEY enviroment is not properly configured.");

            string asignName = string.Empty;

            try
            {
                asignName = await _userService.CreateAsync(model);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, asignName)
                }),
                Expires = DateTime.UtcNow.AddHours(12), // Tiempo de expiración del token
                Issuer = "chopify.com.ar",
                Audience = "chopify.com.ar",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return CreatedAtAction(nameof(Create), new { username = asignName, token = tokenString, expiry = tokenDescriptor.Expires });
        }

        [HttpPost]
        [Authorize]
        [Route("validate-token")]
        public IActionResult ValidateToken()
        {
            return Ok(new { valid = true });
        }
    }
}
