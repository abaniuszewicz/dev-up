using FluentValidation;

namespace DevUp.Application.Organization.Queries.Validators
{
    internal class GetTeamQueryValidator : AbstractValidator<GetTeamQuery>
    {
        public GetTeamQueryValidator()
        {
            RuleFor(q => q.Id)
                .NotEmpty();
        }
    }
}
