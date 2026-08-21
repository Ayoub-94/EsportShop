using EsportShop.Api.DTOs;
using FluentValidation;

namespace EsportShop.Api.Validators
{
    public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Le nom du produit est Obligatoire")
                .MaximumLength(100).WithMessage("Le nom ne peut pas dépasser 100 caractères.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La description est obligatoire.");

            RuleFor(x => x.Price)
                .InclusiveBetween(0.01M, 10000.0M).WithMessage("Le prix doit être compris entre 0.01 et 10000.0.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Le stock ne peut pas être négatif.");
        }
    }
}
