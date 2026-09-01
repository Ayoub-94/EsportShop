using EsportShop.Api.Models;

namespace EsportShop.Api.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken);
        Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(Category category, CancellationToken cancellationToken);
        Task UpdateAsync(Category category, CancellationToken cancellationToken); // Ajouté pour les bonnes pratiques (mise à jour)
        Task DeleteAsync(Category category, CancellationToken cancellationToken); // Ajouté pour les bonnes pratiques (suppression)
    }
}