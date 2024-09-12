namespace chopify.Models
{
    public class SongReadDTO
    {
        public string Id { get; set; } = string.Empty;

        public string Artist { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string FirstReleaseDate { get; set; } = string.Empty;

        public TimeSpan Duration { get; set; }

        public string CoverUrl { get; set; } = string.Empty;
        
        public bool IsSuggested { get; set; } = false;

        public string SuggestedBy {  get; set; } = string.Empty;
    }
}
