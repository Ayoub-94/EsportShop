using EsportShop.Api.DTOs;
using EsportShop.Api.Models;

namespace EsportShop.Api.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync(CancellationToken cancellationToken);
        Task<ProductResponseDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken);
        Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto, CancellationToken cancellationToken);

        Task UpdateProductAsync(int id, ProductUpdateDto dto, CancellationToken cancellationToken);
        Task DeleteProductAsync(int id, CancellationToken cancellationToken);
        Task DeleteAllProductAsync(CancellationToken cancellationToken);
    }
}
