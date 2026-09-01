namespace EsportShop.Api.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IUserRepository Users { get; }
        ICartRepository Carts { get; }

        Task<bool> CompleteAsync(CancellationToken cancellationToken);
    }
}
