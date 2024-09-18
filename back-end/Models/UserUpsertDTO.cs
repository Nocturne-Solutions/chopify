using System.ComponentModel.DataAnnotations;

namespace chopify.Models
{
    public class UserUpsertDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(32, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 32 caracteres.")]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "El nombre solo puede contener letras, sin espacios ni símbolos.")]
        public string Name { get; set; } = string.Empty;
    }
}
