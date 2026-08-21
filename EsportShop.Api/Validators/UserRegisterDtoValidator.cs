using EsportShop.Api.DTOs;
using FluentValidation;
using System.Text;

namespace EsportShop.Api.Validators
{
    public class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
    {
        public UserRegisterDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("L'email est obligatoire")
                .EmailAddress().WithMessage("L'email n'est pas au bon format.")
                .Must(email => !email.EndsWith("@mailinator.com"))
                .WithMessage("Les adresses emails temporaires ne sont pas autorisées.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Le mot de passe est obligatoire.")
                .MinimumLength(8).WithMessage("Le mot de passe doit contenir au moins 8 caractères.")
                // Au moins une lettre majuscule
                .Matches(@"[A-Z]").WithMessage("Le mot de passe doit contenir au moins une lettre majuscule.")
                // Au moins une lettre minuscule
                .Matches(@"[a-z]").WithMessage("Le mot de passe doit contenir au moins une lettre minuscule.")
                // Au moins un chiffre
                .Matches(@"[0-9]").WithMessage("Le mot de passe doit contenir au moins un chiffre.")
                // Au moins un caractère spécial
                .Matches(@"[\@\$\!\%\*\?\&\#]").WithMessage("Le mot de passe doit contenir au moins un caractère spécial.");
        }
    }
}
