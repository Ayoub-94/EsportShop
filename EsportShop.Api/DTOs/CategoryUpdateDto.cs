using System.ComponentModel.DataAnnotations;

namespace EsportShop.Api.DTOs
{
    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "Le nom de la catégorie est obligatoire.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 100 caractères.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
        public string? Description { get; set; }
    }
}
