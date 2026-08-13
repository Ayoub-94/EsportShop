namespace EsportShop.Api.Data // Au lieu de Models
{
    using Microsoft.EntityFrameworkCore;
    using EsportShop.Api.Models; // Importez vos modèles ici

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
    }
}