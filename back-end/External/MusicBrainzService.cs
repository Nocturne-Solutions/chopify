using chopify.Data.Entities;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MongoDB.Bson;

namespace chopify.External
{
    public class MusicBrainzService
    {
        private readonly Query _query;

        private static readonly Lazy<MusicBrainzService> _instance = new(() => new MusicBrainzService());

        private MusicBrainzService()
        {
            _query = new Query("Chopify", "0.1", "test");
        }

        public static MusicBrainzService Instance => _instance.Value;

        public async Task<IEnumerable<Song>> FetchTracksAsync(string search, int limit = 25)
        {
            var query = $"{search}";

            var recordings = await _query.FindRecordingsAsync(query, limit); 

            return recordings.Results.Select(r => MapRecordingToMusic(r.Item));
        }

        private Song MapRecordingToMusic(IRecording recording)
        {
            var artist = recording.ArtistCredit != null && recording.ArtistCredit.Any()
                ? string.Join(" - ", recording.ArtistCredit.Select(ac => ac.Name))
                : "Unknown";
             
            var release = recording.Releases?[recording.Releases.Count - 1];
            var coverUrl = release != null ? $"https://coverartarchive.org/release/{release.Id}/front" : string.Empty;

            return new Song
            {
                Id = MbidToObjectId(recording.Id),
                Name = recording.Title ?? "null",
                Artist = artist,
                FirstReleaseDate = recording.FirstReleaseDate == null ? "Unknown" : recording.FirstReleaseDate.NearestDate.ToShortDateString(),
                Duration = recording.Length ?? TimeSpan.Zero,
                CoverUrl = coverUrl,
            };
        }

        public static ObjectId MbidToObjectId(Guid mbid)
        {
            byte[] guidBytes = mbid.ToByteArray();
            byte[] objectIdBytes = new byte[12];

            Array.Copy(guidBytes, objectIdBytes, 12);

            return new ObjectId(objectIdBytes);
        }

        public static Guid ObjectIdToMbid(ObjectId objectId)
        {
            byte[] objectIdBytes = objectId.ToByteArray();
            byte[] guidBytes = new byte[16];

            Array.Copy(objectIdBytes, guidBytes, 12);

            return new(guidBytes);
        }
    }
}
