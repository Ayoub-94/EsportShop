using EsportShop.Api.Data;
using EsportShop.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EsportShop.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsTracking()
                .FirstOrDefaultAsync(u  => u.Email == email, cancellationToken);
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }
    }
}
