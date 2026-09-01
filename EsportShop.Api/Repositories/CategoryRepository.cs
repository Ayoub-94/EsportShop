using EsportShop.Api.Data;
using EsportShop.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EsportShop.Api.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {

        // 1. Dépendance en lecture seule (readonly)
        // Pourquoi ? 'readonly' garantit qu'une fois le repository instancié, _context ne pourra plus jamais être réassigné par erreur. C'est une sécurité.
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        // 2. Récupérer toutes les catégories
        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken)
        {
            // Pourquoi .AsNoTracking() ? 
            // En entreprise, pour les requêtes en lecture seule (GET), on ajoute .AsNoTracking() 
            // pour dire à EF Core de ne pas garder les objets en mémoire cache. Cela améliore considérablement les performances.
            return await _context.Categories
                .Include(c => c.Products)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // 3. Récupérer une catégorie par son ID
        public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            // Pourquoi 'Category?' avec un point d'interrogation ? 
            // C'est le type nullable de C#. Si l'ID n'existe pas en base, la méthode retourne 'null' proprement.
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        // 4. Ajouter une catégorie
        public async Task AddAsync(Category category, CancellationToken cancellationToken)
        {
            // Pourquoi pas de SaveChanges ici ? 
            // C'est une règle d'or en entreprise (Unit of Work pattern léger). 
            // Le repository prépare l'action en mémoire, et c'est le contrôleur (ou un service) 
            // qui valide l'enregistrement global avec SaveChangesAsync().
            await _context.Categories.AddAsync(category, cancellationToken);
        }

        // 5. Mettre à jour une catégorie (Bonne pratique ajoutée)
        public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
        {
            // Pourquoi cette ligne ? 
            // Elle indique explicitement à Entity Framework que cet objet a été modifié et qu'il doit générer un SQL UPDATE.
            _context.Categories.Update(category);
            await Task.CompletedTask; // Gardé en async pour respecter l'interface
        }

        // 6. Supprimer une catégorie (Bonne pratique ajoutée)
        public async Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
        {
            _context.Categories.Remove(category);
            await Task.CompletedTask;
        }
    }
}