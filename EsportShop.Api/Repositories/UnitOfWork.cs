using EsportShop.Api.Data;

namespace EsportShop.Api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // On initialise nos repositories ici
        public ICategoryRepository Categories { get; }
        public IProductRepository Products { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(AppDbContext context, 
            ICategoryRepository categoryRepository, 
            IProductRepository productRepository,
            IUserRepository userRepository)
        {
            _context = context;
            Categories = categoryRepository;
            Products = productRepository;
            Users = userRepository;
        }

        public async Task<bool> CompleteAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
