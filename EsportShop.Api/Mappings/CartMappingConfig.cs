using EsportShop.Api.DTOs;
using EsportShop.Api.Models;
using Mapster;

namespace EsportShop.Api.Mappings
{
    public class CartMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Configuration pour mapper Cart vers CartDto
            config.NewConfig<Cart, CartDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Items, src => src.Items)
                .Map(dest => dest.TotalPrice, src => src.Items.Sum(i => (i.Product != null ? i.Product.Price : 0)));

            // Configuration pour mapper CartItem vers CartItemDto
            config.NewConfig<CartItem, CartItemDto>()
                .Map(dest => dest.ProductName, src => src.Product != null ? src.Product.Name : "Produit inconnu")
                .Map(dest => dest.UnitPrice, src => src.Product != null ? src.Product.Price : 0);
        }
    }
}
