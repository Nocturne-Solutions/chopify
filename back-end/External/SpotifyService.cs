using chopify.Configurations;
using chopify.Data.Entities;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web;

namespace chopify.External
{
    public class SpotifyService(IOptions<SpotifySettings> settings)
    {
        private readonly SpotifyClient client = new(SpotifyClientConfig.CreateDefault().WithAuthenticator(new ClientCredentialsAuthenticator(settings.Value.ClientId, settings.Value.ClientSecret)));

        public async Task<IEnumerable<FullTrack>> FetchTracksAsync(string search, int limit = 15)
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

            if (playlist == null || playlist.Tracks == null || playlist.Tracks.Items == null)
                return [];

            return playlist.Tracks.Items
                .Where(item => item.Track is FullTrack)
                .Take(30)
                .Select(item => item.Track as FullTrack)
                .Where(track => track != null)!;
        }

        public async Task<FullTrack> GetRandomTrackAsync()
        {
            var tracks = await GetMostPopularTracksArgentinaAsync();

            return tracks.ElementAt(new Random().Next(tracks.Count()));
        }

        public async Task<FullTrack> GetFullTrackById(string id) =>
            await client.Tracks.Get(id);
    }
}
