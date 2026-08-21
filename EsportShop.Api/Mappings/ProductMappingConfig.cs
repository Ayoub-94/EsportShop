using Mapster;
using EsportShop.Api.Models;
using EsportShop.Api.DTOs;

namespace EsportShop.Api.Mappings
{
    public class ProductMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // 1. Comment mapper un Product vers ProductResponseDto
            config.NewConfig<Product, ProductResponseDto>()
                .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : "Aucune catégorie");

            // 2. Comment mapper un ProductCreateDto vers un modèle Product
            config.NewConfig<ProductCreateDto, Product>()
                .Map(dest => dest.Name, src => src.Name.Trim())
                .Map(dest => dest.Description, src => src.Description.Trim());

            // 3. Comment mapper un ProductUpdateDto vers un produit existant
            config.NewConfig<ProductUpdateDto, Product>()
                .Map(dest => dest.Name, src => src.Name.Trim())
                .Map(dest => dest.Description, src => src.Description.Trim());
        }
    }
}