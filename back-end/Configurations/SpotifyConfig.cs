using MongoDB.Driver;

namespace chopify.Configurations
{
    public class SpotifySettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }

    public class SpotifyRedirectUris
    {
        public string LoginCallback { get; set; } = string.Empty;
        public string ErrorCallback { get; set; } = string.Empty;
        public string SuccessCallback { get; set; } = string.Empty;
    }

    public static class SpotifyConfig
    {
        public static IServiceCollection ConfigureSpotify(this IServiceCollection services, IConfiguration configuration)
        {
            var spotifyClientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            var spotifyClientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");

            if (string.IsNullOrWhiteSpace(spotifyClientId))
                throw new ArgumentNullException("SPOTIFY_CLIENT_ID enviroment is not properly configured.");

            if (string.IsNullOrWhiteSpace(spotifyClientSecret))
                throw new ArgumentNullException("SPOTIFY_CLIENT_SECRET enviroment is not properly configured.");

            services.Configure<SpotifySettings>(options =>
            {
                options.ClientId = spotifyClientId;
                options.ClientSecret = spotifyClientSecret;
            });

            services.Configure<SpotifyRedirectUris>(options =>
            {
                options.LoginCallback = "http://localhost:5140/spotify-auth/callback";
                options.ErrorCallback = "http://localhost:5500/admin/src/pages/error.html";
                options.SuccessCallback = "http://localhost:5500/admin/src/pages/success.html";
            });

            return services;
        }
    }
}
