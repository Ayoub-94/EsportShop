using EsportShop.Api.DTOs;
using EsportShop.Api.Models;
using EsportShop.Api.Repositories;
using Mapster;

namespace EsportShop.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();

            return products.Adapt<IEnumerable<ProductResponseDto>>();
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if(product == null) return null;

            return product.Adapt<ProductResponseDto>();
        }

        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
            if(category == null)
            {
                throw new ArgumentException($"La catégorie avec l'ID {dto.CategoryId} n'existe pas");
            }

            var product = dto.Adapt<Product>();

            await _unitOfWork.Products.AddAsync(product);
            var succes = await _unitOfWork.CompleteAsync();

            if(!succes)
            {
                throw new Exception("Erreur lors de l'enregistrement du produit en base de données.");
            }

           var responseDto = product.Adapt<ProductResponseDto>();
            responseDto.CategoryName = category.Name;

            return responseDto;
        }

        // 4. UPDATE (Mettre à jour un produit)
        public async Task UpdateProductAsync(int id, ProductUpdateDto dto)
        {
            // Récupérer le produit existant 
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if(product == null)
            {
                throw new KeyNotFoundException($"Le produit avec l'ID {id} est introuvable.");
            }

            dto.Adapt(product);

            _unitOfWork.Products.UpdateAsync(product);
            var success = await _unitOfWork.CompleteAsync();

            if(!success)
            {
                throw new Exception("Erreur lors de la mise à jour du produit en base de données.");
            }
        }

        // 5. DELETE (Supprimer un produit)
        public async Task DeleteProductAsync(int id)
        {
            // Récupérer le produit existant
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Le produit avec l'ID {id} est introuvable.");
            }

            // Supprimer le produit
            _unitOfWork.Products.DeleteAsync(product);
            var success = await _unitOfWork.CompleteAsync();

            if(!success)
            {
                throw new Exception("Erreur lors de la suppression du produit en base de données.");
            }
        }

        public async Task DeleteAllProductAsync()
        {
            var allProducts = await _unitOfWork.Products.GetAllAsync();

            if (!allProducts.Any()) return;

            foreach (var product in allProducts)
            {
                _unitOfWork.Products.DeleteAsync(product);
            }

            if (!await _unitOfWork.CompleteAsync())
            {
                throw new Exception("Erreur lors de la suppression de tous les produits.");

            }
        }
    }
}
