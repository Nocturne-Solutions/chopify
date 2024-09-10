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
        public async Task<IEnumerable<FullTrack>> FetchTracksAsync(string search, int limit = 25)
        {
            SearchRequest query = new(SearchRequest.Types.Track, search)
            {
                Limit = limit
            };

            var result = await client.Search.Item(query);

            if (result == null || result.Tracks == null || result.Tracks.Items == null)
                return [];

            return result.Tracks.Items;
        }

        public async Task<IEnumerable<FullTrack>> GetMostPopularTracksArgentinaAsync()
        {
            var playlist = await client.Playlists.Get("37i9dQZEVXbMMy2roB9myp");

            var fullTracks = new List<FullTrack>();

            if (playlist == null || playlist.Tracks == null || playlist.Tracks.Items == null)
                return [];

            foreach (PlaylistTrack<IPlayableItem> item in playlist.Tracks.Items)
                if (item.Track is FullTrack track && track != null)
                    fullTracks.Add(track);

            return fullTracks;
        }
    }
}
