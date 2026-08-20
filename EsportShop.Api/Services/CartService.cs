using EsportShop.Api.DTOs;
using EsportShop.Api.Models;
using EsportShop.Api.Repositories;
using Microsoft.EntityFrameworkCore.Metadata;

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
        public async Task<CartDto> GetCartByUserIdAsync(int userId)
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);

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
            return new CartDto
            {
                Id = cart.UserId,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "Produit inconnu",
                    UnitPrice = i.Product?.Price ?? 0,
                    Quantity = i.Quantity
                }).ToList(),
                TotalPrice = cart.Items.Sum(i => (i.Product?.Price ?? 0) * i.Quantity)
            };
        }

        // 2. Ajouter un produit au panier (ou incrémenter la quantité s'il existe déjà)
        public async Task AddToCartAsync(int userId, AddToCartDto request)
        {
            // Vérification métier : Le produit existe-t-il dans le catalogue ?
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);

            if(product == null)
            {
                throw new KeyNotFoundException($"Le produit avec l'ID {request.ProductId} est introuvable.");
            }
            // Vérification métier optionnelle : Le stock est-il suffisant ?
            if (product.Stock < request.Quantity)
            {
                throw new InvalidOperationException($"Stock insuffisant pour le produit '{product.Name}'. Stock disponible : {product.Stock}");
            }

            // Récupérer le panier de l'utilisateur
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);

            // Si l'utilisateur n'a pas de panier, on en crée un nouveau
            if (cart == null)
            {
                cart = new Models.Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>()
                };
                await _unitOfWork.Carts.AddAsync(cart);
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
            await _unitOfWork.CompleteAsync();
        }

        // 3. Modifier directement la quantité d'un article

        public async Task UpdateItemQuantityAsync(int userId, int productId, int newQuantity)
        {
            if (newQuantity <= 0)
            {
                // Si la quantité est 0 ou moins, on supprime l'article
                await RemoveItemAsync(userId, productId);
                return;
            }

            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                throw new KeyNotFoundException("Aucun panier trouvé pour cet utilisateur.");
            }

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                throw new KeyNotFoundException("Cet article n'est pas dans votre panier.");
            }

            item.Quantity = newQuantity;
            await _unitOfWork.CompleteAsync();
        }

        // 4. Supprimer un article spécifique du panier
        public async Task RemoveItemAsync(int userId, int productId)
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            if(cart == null)return;

            var item = cart.Items.FirstOrDefault(i =>i.ProductId == productId);
            if (item != null)
            {
                {
                    _unitOfWork.Carts.DeleteItemAsync(item);
                    await _unitOfWork.CompleteAsync();
                }
            }
        }

        // 5. Vider entièrement le panier
        public async Task ClearCartAsync(int userId)
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                _unitOfWork.Carts.DeleteAsync(cart);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
