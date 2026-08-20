using EsportShop.Api.DTOs;

namespace EsportShop.Api.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartByUserIdAsync(int  userId);
        Task AddToCartAsync(int userId, AddToCartDto request);
        Task UpdateItemQuantityAsync(int userId, int productId, int newQuantity);
        Task RemoveItemAsync(int userId, int productId);
        Task ClearCartAsync(int userId);
    }
}
