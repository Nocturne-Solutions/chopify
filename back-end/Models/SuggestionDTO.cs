using System.ComponentModel.DataAnnotations;

namespace chopify.Models
{
    public class SuggestionDTO
    {
        [Required(ErrorMessage = "La canción es obligatoria.")]
        public string SpotifySongId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string SuggestedBy { get; set; } = string.Empty;
    }
}
