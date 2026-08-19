
using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.Models.Dtos
{
    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "El nombre de categoria es obligatorio!")]
        [MaxLength(50, ErrorMessage = "El nombre de categoria no puede superar los 50 caracteres!")]
        [MinLength(5, ErrorMessage = "El nombre de categoria debe tener al menos 5 caracteres!")]
        public string Name { get; set; } = string.Empty;
    }
}