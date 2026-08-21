using EsportShop.Api.DTOs;
using FluentValidation;

namespace EsportShop.Api.Validators
{
    public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
    {
        public UserLoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("L'email est obligatoire.")
                .EmailAddress().WithMessage("L'email n'est pas au bon format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Le mot de passe est obligatoire.");
        }
    }
}
