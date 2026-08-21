using EsportShop.Api.DTOs;
using FluentValidation;

namespace EsportShop.Api.Validators
{
    public class AddToCartDtoValidator : AbstractValidator<AddToCartDto>
    {
        public AddToCartDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("L'identifiant du produit est obligatoire.");

            RuleFor(x => x.Quantity)
                .InclusiveBetween(1, 100).WithMessage("La quantité doit être compris entre 1 et 100.");
        }
    }
}
