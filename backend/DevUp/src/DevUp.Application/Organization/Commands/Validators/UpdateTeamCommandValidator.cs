using FluentValidation;

namespace DevUp.Application.Organization.Commands.Validators
{
    public class UpdateTeamCommandValidator : AbstractValidator<UpdateTeamCommand>
    {
        public UpdateTeamCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty();

            RuleFor(c => c.Name)
                .SetValidator(new TeamNameValidator<UpdateTeamCommand>());
        }
    }
}
