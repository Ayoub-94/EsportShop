using EsportShop.Api.DTOs;
using EsportShop.Api.Models;
using EsportShop.Api.Repositories;

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

            return products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CategoryName = p.Category?.Name ?? "Aucune catégorie"
            });
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if(product == null) return null;

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryName = product.Category?.Name ?? "Aucune catégorie"
            };
        }

        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto dto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
            if(category == null)
            {
                throw new ArgumentException($"La catégorie avec l'ID {dto.CategoryId} n'existe pas");
            }

            var product = new Product
            {
                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),
                Price = dto.Price,
                Stock = dto.Stock,
                CategoryId = category.Id,
            };

            await _unitOfWork.Products.AddAsync(product);
            var succes = await _unitOfWork.CompleteAsync();

            if(!succes)
            {
                throw new Exception("Erreur lors de l'enregistrement du produit en base de données.");
            }

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryName = category.Name
            };
        }
    }
}
