using EsportShop.Api.Models;

namespace EsportShop.Api.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken);
        Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(Product product, CancellationToken cancellationToken);
        Task UpdateAsync(Product product, CancellationToken cancellationToken);
        Task DeleteAsync(Product product, CancellationToken cancellationToken);
    }
}
