using EsportShop.Api.DTOs;
using EsportShop.Api.Models;
using Mapster;

namespace EsportShop.Api.Mappings
{
    public class CategoryMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Category, CategoryResponseDto>();
        }
    }
}
