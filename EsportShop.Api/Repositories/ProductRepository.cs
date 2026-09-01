using EsportShop.Api.Data;
using EsportShop.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EsportShop.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Products 
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }
        public async Task AddAsync(Product product, CancellationToken cancellationToken)
        {
             await _context.Products.AddAsync(product, cancellationToken);
        }

        public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
        {
            _context.Products.Update(product);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
        {
            _context.Products.Remove(product);
            await Task.CompletedTask;
        }
    }
}
