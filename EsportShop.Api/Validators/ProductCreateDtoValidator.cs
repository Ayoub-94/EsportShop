using EsportShop.Api.DTOs;
using FluentValidation;

namespace EsportShop.Api.Validators
{
    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Le nom du produit est obligatoire.")
                .MaximumLength(100).WithMessage("Le nom ne peut pas dépasser 100 caractères.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La description est obligatoire.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Le prix doit être supérieur à 0.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Le stock ne peut pas être négatif.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("L'identifiant de la catégorie est obligatoire et doit être valide.");
        }
    }
}
