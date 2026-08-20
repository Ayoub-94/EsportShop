using System.ComponentModel.DataAnnotations;

namespace EsportShop.Api.DTOs
{
    public class ProductUpdateDto
    {
        [Required(ErrorMessage = "Le nom du produit est obligatoire")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La description est obligatoire.")]
        public string Description { get; set; }

        [Range(0.01, 10000.0, ErrorMessage = "Le prix doit être supérieur à 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Le stock ne peut pas être négatif.")]
        public int Stock { get; set; }
    }
}
