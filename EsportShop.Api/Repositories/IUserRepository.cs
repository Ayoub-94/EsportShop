using EsportShop.Api.Models;

namespace EsportShop.Api.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);    
        Task AddAsync(User user, CancellationToken cancellationToken);
    }
}
