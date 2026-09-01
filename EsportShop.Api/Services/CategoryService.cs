using EsportShop.Api.DTOs;
using EsportShop.Api.Models;
using EsportShop.Api.Repositories;
using Mapster;

namespace EsportShop.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync(CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);


            return categories.Adapt<IEnumerable<CategoryResponseDto>>();
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
            if (category == null) return null; // Ou lever une exception personnalisée NotFoundException
                
            return category.Adapt<CategoryResponseDto>();
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto dto, CancellationToken cancellationToken)
        {
            // EXIGENCE MÉTIER 1 : Validation des données entrantes
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Le nom de la catégorie est obligatoire.");
            }

            // EXIGENCE MÉTIER 2 : Nettoyage des données
            var category = dto.Adapt<Category>();
            category.Name = dto.Name.Trim();
            category.Description = dto.Description?.Trim() ?? string.Empty;

            // Appel au repository
            await _unitOfWork.Categories.AddAsync(category, cancellationToken);

            var succes = await _unitOfWork.CompleteAsync(cancellationToken);
            if(!succes)
            {
                throw new Exception("Erreur lors de l'enregistrement de la catégorie.");
            }

            return category.Adapt<CategoryResponseDto>();
        }

        public async Task UpdateCategoryAsync(int id, CategoryUpdateDto dto, CancellationToken cancellationToken)
        {
            // 1. Récupérer la catégorie existante
            var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
            if (category == null)
            {
                throw new KeyNotFoundException($"La catégorie avec l'ID {id} est introuvable.");
            }

            // 2. Validation métier (si nécessaire)
            if (string.IsNullOrWhiteSpace(dto?.Name))
            {
                throw new ArgumentException("Le nom de la catégorie est obligatoire.");
            }

            // 3. Mise à jour des propriétés (avec nettoyage)
            category.Name = dto.Name.Trim();
            category.Description = dto.Description?.Trim() ?? string.Empty;

            // 4. Appel au repository pour signaler la modification
            await _unitOfWork.Categories.UpdateAsync(category, cancellationToken);

            // 5. Sauvegarde via l'Unit of Work
            var success = await _unitOfWork.CompleteAsync(cancellationToken);
            if (!success)
            {
                throw new Exception("Erreur lors de la mise à jour de la catégorie.");
            }
        }

        public async Task DeleteCategoryAsync(int id, CancellationToken cancellationToken)
        {
            // 1. Récupérer la catégorie existante
            var category =await  _unitOfWork.Categories.GetByIdAsync(id, cancellationToken); 
            if(category == null)
            {
                throw new KeyNotFoundException($"La catégorie avec l'ID {id} est introuvable.");
            }

            // 2. Suppression via le repository
             await _unitOfWork.Categories.DeleteAsync(category, cancellationToken);

            // 3. Ne pas oublier de valider les changements via l'Unit of Work !
            var success = await _unitOfWork.CompleteAsync(cancellationToken);
            if(!success)
            {
                throw new Exception("Erreur lors de la suppression de la catégorie.");
            }
        }
    }
}