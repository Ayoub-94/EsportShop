using EsportShop.Api.DTOs;
using EsportShop.Api.Models;

namespace EsportShop.Api.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto);

        Task UpdateProductAsync(int id, ProductUpdateDto dto);
        Task DeleteProductAsync(int id);
        Task DeleteAllProductAsync();
    }
}
