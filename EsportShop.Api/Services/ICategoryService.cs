using EsportShop.Api.DTOs;
using EsportShop.Api.Models;

namespace EsportShop.Api.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync(CancellationToken cancellationToken);
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken);
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto dto, CancellationToken cancellationToken);
        Task UpdateCategoryAsync(int id, CategoryUpdateDto dto, CancellationToken cancellationToken);
        Task DeleteCategoryAsync(int id, CancellationToken cancellationToken);
    }
}