using chopify.Data.Entities;
using SpotifyAPI.Web;

namespace chopify.External
{
    public class SpotifyService
    {
        private readonly SpotifyClient client;

        private static readonly Lazy<SpotifyService> _instance = new(() => new SpotifyService());

        private SpotifyService()
        {
            var spotifyClientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            var spotifyClientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");

            if (string.IsNullOrWhiteSpace(spotifyClientId))
                throw new ArgumentNullException("SPOTIFY_CLIENT_ID enviroment is not properly configured.");

            if (string.IsNullOrWhiteSpace(spotifyClientSecret))
                throw new ArgumentNullException("SPOTIFY_CLIENT_SECRET enviroment is not properly configured.");

            var config = SpotifyClientConfig
              .CreateDefault()
              .WithAuthenticator(new ClientCredentialsAuthenticator(spotifyClientId, spotifyClientSecret));

            client = new SpotifyClient(config);
        }

        public static SpotifyService Instance => _instance.Value;
        public async Task<IEnumerable<Music>> FetchTracksAsync(string search, int limit = 25)
        {
            SearchRequest query = new(SearchRequest.Types.Track, search)
            {
                Limit = limit
            };

            var result = await client.Search.Item(query);

            if (result == null || result.Tracks == null || result.Tracks.Items == null)
                return [];

            return result.Tracks.Items.Select(MapTrackToMusic) ?? [];
        }

        private Music MapTrackToMusic(FullTrack track)
        {
            var artist = track.Artists != null && track.Artists.Count != 0
                ? string.Join(" - ", track.Artists.Select(ac => ac.Name))
                : "Unknown";

            return new Music
            {
                SpotifyId = track.Id,
                Name = track.Name,
                Artist = artist,
                FirstReleaseDate = track.Album.ReleaseDate,
                Duration = new TimeSpan(0, 0, 0, 0, track.DurationMs),
                CoverUrl = track.Album.Images[0].Url
            };
        }
    }
}
