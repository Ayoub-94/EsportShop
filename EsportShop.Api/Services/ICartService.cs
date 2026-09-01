using EsportShop.Api.DTOs;

namespace EsportShop.Api.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartByUserIdAsync(int  userId, CancellationToken cancellationToken);
        Task AddToCartAsync(int userId, AddToCartDto request, CancellationToken cancellationToken);
        Task UpdateItemQuantityAsync(int userId, int productId, int newQuantity, CancellationToken cancellationToken);
        Task RemoveItemAsync(int userId, int productId, CancellationToken cancellationToken);
        Task ClearCartAsync(int userId, CancellationToken cancellationToken);
    }
}
