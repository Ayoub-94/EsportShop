using System.ComponentModel.DataAnnotations;

namespace EsportShop.Api.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "L'identifiant de l'utilisateur est obligatoire.")]
        public int UserId { get; set; }

        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
