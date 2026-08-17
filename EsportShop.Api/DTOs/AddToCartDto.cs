using System.ComponentModel.DataAnnotations;

namespace EsportShop.Api.DTOs
{
    public class AddToCartDto
    {
        [Required(ErrorMessage = "L'identifiant du produit est obligatoire")]
        public int ProductId { get; set; }

        [Range(1, 100, ErrorMessage = "La quantité doit être comprise entre 1 et 100.")]
        public int Quantity { get; set; }
    }
}
