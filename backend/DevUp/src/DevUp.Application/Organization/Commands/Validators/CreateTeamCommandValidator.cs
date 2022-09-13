using FluentValidation;

namespace DevUp.Application.Organization.Commands.Validators
{
    internal class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
    {
        public CreateTeamCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty();

            RuleFor(c => c.Name)
                .NotEmpty()
                .Length(3, 50);

            When(c => !string.IsNullOrEmpty(c.Name), () =>
            {
                RuleFor(c => c.Name)
                    .Matches(@"^[a-zA-Z\-\.,_ ]+$").WithMessage("'{PropertyName}' may only contain letters, hyphens, dots, commas, underscores and spaces.")
                    .Must(s => char.IsLetter(s.First())).WithMessage("'{PropertyName}' must begin with a letter.")
                    .Must(s => char.IsLetter(s.Last())).WithMessage("'{PropertyName}' must end with a letter.")
                    .Must(s => !s.Contains("  ")).WithMessage("'{PropertyName}' cannot contain two consecutive spaces.")
                    .Matches(@"[\-\.,_]{2}").WithMessage("'{PropertyName}' cannot contain two consecutive symbols.");
            });
        }
    }
}
