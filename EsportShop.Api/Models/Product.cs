using System.ComponentModel.DataAnnotations;

namespace EsportShop.Api.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de l'article de sport est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le nom ne doit pas dépasser 100 caractères.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La description ne doit pas dépasser 500 caractères.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 10000.00, ErrorMessage = "Le prix doit être compris entre 0.01 € et 10 000 €.")]
        public decimal Price { get; set; }

        [Range(0, 1000, ErrorMessage = "Le stock disponible doit être compris entre 0 et 1000.")]
        public int Stock { get; set; }

        // Clé étrangère vers la catégorie
        public int CategoryId { get; set; }

        // Propriété de navigation
        public Category? Category { get; set; }
    }
}