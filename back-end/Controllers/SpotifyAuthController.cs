using chopify.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web;

namespace chopify.Controllers
{
    [ApiController]
    [Route("spotify-auth")]
    public class SpotifyAuthController(IOptions<SpotifySettings> settings, IOptions<SpotifyRedirectUris> redirectUris) : ControllerBase
    {
        private readonly SpotifySettings _spotifySettings = settings.Value;
        private readonly SpotifyRedirectUris _spotifyRedirectUris = redirectUris.Value;

        [HttpGet("login")]
        public IActionResult Login()
        {
            var loginRequest = new LoginRequest(
                new Uri(_spotifyRedirectUris.LoginCallback),
                _spotifySettings.ClientId,
                LoginRequest.ResponseType.Code
            )
            {
                Scope = [
                    Scopes.UserReadPrivate,
                    Scopes.UserReadEmail
                ]
            };

            var authUri = loginRequest.ToUri();

            return Redirect(authUri.ToString());
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string? code, [FromQuery] string? error)
        {
            if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(code))
                return Redirect(_spotifyRedirectUris.ErrorCallback);

            try
            {
                var oauthClient = new OAuthClient();

                var tokenRequest = new AuthorizationCodeTokenRequest(
                    _spotifySettings.ClientId,
                    _spotifySettings.ClientSecret,
                    code,
                    new Uri(_spotifyRedirectUris.LoginCallback)
                );

                var token = await oauthClient.RequestToken(tokenRequest);

                if (token == null)
                    return Redirect(_spotifyRedirectUris.ErrorCallback);

                //Guardar token en base de datos o en memoria. Fijarse el tema del hardcodeo de los urls

                return Redirect(_spotifyRedirectUris.SuccessCallback);
            }
            catch
            {
                return Redirect(_spotifyRedirectUris.ErrorCallback);
            }
        }
    }
}
