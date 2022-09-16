using FluentValidation;

namespace DevUp.Application.Organization.Commands.Validators
{
    internal class DeleteTeamCommandValidator : AbstractValidator<DeleteTeamCommand>
    {
        public DeleteTeamCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty();
        }
    }
}
