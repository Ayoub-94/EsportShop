using EsportShop.Api.Models;

namespace EsportShop.Api.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);    
        Task AddAsync(User user);
    }
}
