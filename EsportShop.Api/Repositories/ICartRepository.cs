using EsportShop.Api.Models;

namespace EsportShop.Api.Repositories
{
    public interface ICartRepository
    {
        // Récupérer le panier d'un utilisateur avec ses articles et produits associés
        Task<Cart?> GetCartByUserIdAsync(int userId);

        // Créer un nouveau panier pour un utilisateur
        Task AddAsync(Cart cart);

        // Supprimer un article spécifique du panier (CartItem)
        Task DeleteItemAsync(CartItem cartItem);

        // Vider entièrement le panier (supprimer tous les articles ou le panier lui-même)
        Task DeleteAsync(Cart cart);
    }
}
