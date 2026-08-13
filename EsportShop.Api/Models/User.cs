using System.ComponentModel.DataAnnotations;

namespace EsportShop.Api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "L'adresse e-mail est obligatoire.")]
        [EmailAddress(ErrorMessage = "Le format de l'e-mail est invalide.")]
        [MaxLength(256, ErrorMessage = "L'e-mail ne peut pas dépasser 256 caractères.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        // Le PasswordHash généré par un service de hachage (ex: BCrypt ou Identity) fait généralement 60 caractères ou plus
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Customer";

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    }
}
