using FluentValidation;

namespace DevUp.Application.Identity.Commands.Validators
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(c => c.Username)
                .NotEmpty()
                .Length(6, 30);

            When(c => c.Username is not null, () =>
            {
                RuleFor(c => c.Username)
                    .Matches(@"^[a-z\-]+$").WithMessage("'{PropertyName}' may only contain lowercase letters or hyphens.")
                    .Must(s => !s.StartsWith('-')).WithMessage("'{PropertyName}' cannot begin with a hyphen.")
                    .Must(s => !s.EndsWith('-')).WithMessage("'{PropertyName}' cannot end with a hyphen.")
                    .Must(s => !s.Contains("--")).WithMessage("'{PropertyName}' cannot contain two consecutive hyphens.");
            });

            RuleFor(c => c.Password)
                .NotEmpty()
                .Length(8, 50);

            When(c => c.Password is not null, () =>
            {
                RuleFor(c => c.Password)
                    .Must(s => s.Any(char.IsLower)).WithMessage("'{PropertyName}' must contain at least one lower case letter.")
                    .Must(s => s.Any(char.IsUpper)).WithMessage("'{PropertyName}' must contain at least one upper case letter.")
                    .Must(s => s.Any(char.IsDigit)).WithMessage("'{PropertyName}' must contain at least one digit.")
                    .Must(s => s.Any(c => !char.IsLetterOrDigit(c))).WithMessage("'{PropertyName}' must contain at least one special character.");
            });
        }
    }
}
