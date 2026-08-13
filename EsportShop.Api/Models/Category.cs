using System.ComponentModel.DataAnnotations;

namespace EsportShop.Api.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la catégorie est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le nom ne doit pas dépasser 100 caractères.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La description ne doit pas dépasser 500 caractères.")]
        public string Description { get; set; } = string.Empty;

        // Relation : Une catégorie possède une liste de produits
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}