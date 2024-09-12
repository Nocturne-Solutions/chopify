using System.ComponentModel.DataAnnotations;

namespace chopify.Models
{
    public class VoteUpsertDTO
    {
        [Required(ErrorMessage = "El id de la canción es obligatorio.")]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string User { get; set; } = string.Empty;
    }
}
