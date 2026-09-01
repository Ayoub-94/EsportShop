using EsportShop.Api.Models;

namespace EsportShop.Api.Repositories
{
    public interface ICartRepository
    {
        // Récupérer le panier d'un utilisateur avec ses articles et produits associés
        Task<Cart?> GetCartByUserIdAsync(int userId, CancellationToken cancellationToken);

        // Créer un nouveau panier pour un utilisateur
        Task AddAsync(Cart cart, CancellationToken cancellationToken);

        // Supprimer un article spécifique du panier (CartItem)
        Task DeleteItemAsync(CartItem cartItem, CancellationToken cancellationToken);

        // Vider entièrement le panier (supprimer tous les articles ou le panier lui-même)
        Task DeleteAsync(Cart cart, CancellationToken cancellationToken);
    }
}
