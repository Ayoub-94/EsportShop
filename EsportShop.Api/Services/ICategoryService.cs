using EsportShop.Api.DTOs;
using EsportShop.Api.Models;

namespace EsportShop.Api.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto?> GetCategoryByIdAsync(int id);
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto dto);
        Task UpdateCategoryAsync(int id, CategoryUpdateDto dto);
        Task DeleteCategoryAsync(int id);
    }
}