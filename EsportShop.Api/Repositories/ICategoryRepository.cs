using EsportShop.Api.Models;

namespace EsportShop.Api.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category); // Ajouté pour les bonnes pratiques (mise à jour)
        Task DeleteAsync(Category category); // Ajouté pour les bonnes pratiques (suppression)
    }
}