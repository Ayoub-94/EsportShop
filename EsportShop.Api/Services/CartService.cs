using EsportShop.Api.DTOs;
using EsportShop.Api.Models;
using EsportShop.Api.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Threading;

namespace EsportShop.Api.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. Récupérer et mapper le panier pour l'affichage
        public async Task<CartDto> GetCartByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId, cancellationToken);

            if(cart == null)
            {
                // Si l'utilisateur n'a pas encore de panier, on retourne un DTO vide propre
                return new CartDto
                {
                    Id = 0,
                    Items = new List<CartItemDto>(),
                    TotalPrice = 0
                };                
            }

            // Mapping du modèle de données vers le DTO d'affichage
            return cart.Adapt<CartDto>();
        }

        // 2. Ajouter un produit au panier (ou incrémenter la quantité s'il existe déjà)
        public async Task AddToCartAsync(int userId, AddToCartDto request, CancellationToken cancellationToken)
        {
            // 1. On lance les deux tâches en parallèle sans faire "await" tout de suite
            var productTask = _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
            var cartTask = _unitOfWork.Carts.GetCartByUserIdAsync(userId, cancellationToken);

            // 2. On attend que les deux requêtes se terminent en même temps
            await Task.WhenAll(productTask, cartTask);

            // 3. On récupère les résultats une fois les tâches finies
            var product = await productTask;
            var cart = await cartTask;

            // Vérification métier : Le produit existe-t-il dans le catalogue ?
            if (product == null)
            {
                throw new KeyNotFoundException($"Le produit avec l'ID {request.ProductId} est introuvable.");
            }

            // Vérification métier : Le stock est-il suffisant ?
            if (product.Stock < request.Quantity)
            {
                throw new InvalidOperationException($"Stock insuffisant pour le produit '{product.Name}'. Stock disponible : {product.Stock}");
            }

            // Si l'utilisateur n'a pas de panier, on en crée un nouveau
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>()
                };
                await _unitOfWork.Carts.AddAsync(cart, cancellationToken);
            }

            // Vérifier si le produit est déjà présent dans le panier
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                });
            }

            // Validation globale de la transaction via le Unit of Work
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        // 3. Modifier directement la quantité d'un article

        public async Task UpdateItemQuantityAsync(int userId, int productId, int newQuantity, CancellationToken cancellationToken)
        {
            if (newQuantity <= 0)
            {
                // Si la quantité est 0 ou moins, on supprime l'article
                await RemoveItemAsync(userId, productId);
                return;
            }

            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId, cancellationToken);
            if (cart == null)
            {
                throw new KeyNotFoundException("Aucun panier trouvé pour cet utilisateur.");
            }

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
            {
                throw new KeyNotFoundException("Cet article n'est pas dans votre panier.");
            }

            item.Quantity = newQuantity;
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        // 4. Supprimer un article spécifique du panier
        public async Task RemoveItemAsync(int userId, int productId, CancellationToken cancellationToken = default)
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId, cancellationToken);
            if(cart == null) return;

            var item = cart.Items.FirstOrDefault(i =>i.ProductId == productId);
            if (item != null)
            {
                {
                    await _unitOfWork.Carts.DeleteItemAsync(item, cancellationToken);
                    await _unitOfWork.CompleteAsync(cancellationToken);
                }
            }
        }

        // 5. Vider entièrement le panier
        public async Task ClearCartAsync(int userId, CancellationToken cancellationToken)
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId, cancellationToken);
            if (cart != null)
            {
                await _unitOfWork.Carts.DeleteAsync(cart, cancellationToken);
                await _unitOfWork.CompleteAsync(cancellationToken);
            }
        }
    }
}
