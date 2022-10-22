using FluentValidation;

namespace DevUp.Application.Organization.Commands.Validators
{
    public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
    {
        public CreateTeamCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty();

            RuleFor(c => c.Name)
                .SetValidator(new TeamNameValidator<CreateTeamCommand>());
        }
    }
}
