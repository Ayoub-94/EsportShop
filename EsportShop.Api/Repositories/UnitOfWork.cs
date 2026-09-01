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
        public ICartRepository Carts { get; }

        public UnitOfWork(AppDbContext context, 
            ICategoryRepository categoryRepository, 
            IProductRepository productRepository,
            IUserRepository userRepository,
            ICartRepository cartRepository)
        {
            _context = context;
            Categories = categoryRepository;
            Products = productRepository;
            Users = userRepository;
            Carts = cartRepository;
        }

        public async Task<bool> CompleteAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
