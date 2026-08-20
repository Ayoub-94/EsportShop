using EsportShop.Api.DTOs;
using EsportShop.Api.Models;
using EsportShop.Api.Repositories;

namespace EsportShop.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();


            // Mapping manuel (ou via AutoMapper en entreprise) des Entités vers les DTOs de sortie
            return categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });
        }

        public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return null; // Ou lever une exception personnalisée NotFoundException
                
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto dto)
        {
            // EXIGENCE MÉTIER 1 : Validation des données entrantes
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Le nom de la catégorie est obligatoire.");
            }

            // EXIGENCE MÉTIER 2 : Nettoyage des données
            var category = new Category
            {
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim()
            };

            // Appel au repository
            await _unitOfWork.Categories.AddAsync(category);

            var succes = await _unitOfWork.CompleteAsync();
            if(!succes)
            {
                throw new Exception("Erreur lors de l'enregistrement de la catégorie.");
            }

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task UpdateCategoryAsync(int id, CategoryUpdateDto dto)
        {
            // 1. Récupérer la catégorie existante
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
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
            await _unitOfWork.Categories.UpdateAsync(category);

            // 5. Sauvegarde via l'Unit of Work
            var success = await _unitOfWork.CompleteAsync();
            if (!success)
            {
                throw new Exception("Erreur lors de la mise à jour de la catégorie.");
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            // 1. Récupérer la catégorie existante
            var category =await  _unitOfWork.Categories.GetByIdAsync(id); 
            if(category == null)
            {
                throw new KeyNotFoundException($"La catégorie avec l'ID {id} est introuvable.");
            }

            // 2. Suppression via le repository
             await _unitOfWork.Categories.DeleteAsync(category);

            // 3. Ne pas oublier de valider les changements via l'Unit of Work !
            var success = await _unitOfWork.CompleteAsync();
            if(!success)
            {
                throw new Exception("Erreur lors de la suppression de la catégorie.");
            }
        }
    }
}