using EsportShop.Api.DTOs;
using FluentValidation;

namespace EsportShop.Api.Validators
{
    public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
    {
        public CategoryCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Le nom de la catégorie est obligatoire.")
                .Length(2, 100).WithMessage("Le nom doit contenir entre 2 et 100 caractères.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("La description ne peut pas dépasser 500 caractères.")
                .When(x => x.Description != null);
        }
    }
}
