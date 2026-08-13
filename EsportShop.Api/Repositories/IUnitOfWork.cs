namespace EsportShop.Api.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IUserRepository Users { get; }

        Task<bool> CompleteAsync();
    }
}
