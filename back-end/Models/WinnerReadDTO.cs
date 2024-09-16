namespace chopify.Models
{
    public class WinnerReadDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public string SuggestedBy { get; set; } = string.Empty;
        public int Votes { get; set; }
    }
}
