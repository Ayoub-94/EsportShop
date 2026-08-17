using System.ComponentModel.DataAnnotations;

namespace EsportShop.Api.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "L'identifiant du panier est obligatoire.")]
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        [Required(ErrorMessage = "L'identifiant du produit est obligatoire.")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Range(1, 100, ErrorMessage = "La quantité doit être comprise entre 1 et 100.")]
        public int Quantity { get; set; }
    }
}
