using EsportShop.Api.Data;
using EsportShop.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EsportShop.Api.Repositories
{
    public class CartRepository : ICartRepository
    {

        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        // 2. Récupérer le panier d'un utilisateur par son ID utilisateur
        public async Task<Cart?> GetCartByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            // Pourquoi .Include() et .ThenInclude() ?
            // Pour charger en même temps les articles du panier (CartItems) et les produits associés (Product),
            // évitant ainsi le problème des requêtes "Lazy Loading" non désirées.
            return await _context.carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c  => c.UserId == userId, cancellationToken);
        }

        // 3. Ajouter un nouveau panier
        public async Task AddAsync(Cart cart, CancellationToken cancellationToken)
        {
            await _context.carts .AddAsync(cart, cancellationToken);
        }

        // 4. Supprimer un article spécifique du panier (CartItem)
        public async Task DeleteItemAsync(CartItem cartItem, CancellationToken cancellationToken = default) 
        {
            _context.CartItems.Remove(cartItem);
            await Task.CompletedTask;
        }

        // 5. Supprimer un panier entier
        public async Task DeleteAsync(Cart cart, CancellationToken cancellationToken = default)
        {
            _context.carts.Remove(cart);
            await Task.CompletedTask;
        }
    }
}
