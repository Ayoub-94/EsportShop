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
    }
}