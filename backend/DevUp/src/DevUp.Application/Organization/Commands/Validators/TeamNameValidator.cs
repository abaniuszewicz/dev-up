using System.Text.RegularExpressions;
using DevUp.Domain.Common.Extensions;
using FluentValidation;
using FluentValidation.Validators;

namespace DevUp.Application.Organization.Commands.Validators
{
    public class TeamNameValidator<T> : PropertyValidator<T, string>
    {
        public override string Name => nameof(TeamNameValidator<T>);

        public override bool IsValid(ValidationContext<T> context, string teamName)
        {
            ValidateWithBuiltInValidators(context, teamName);
            ValidateWithCustomValidators(context, teamName);

            // return "valid" so that we don't get additional message for this composite validator
            return true;
        }

        private static void ValidateWithBuiltInValidators(ValidationContext<T> context, string teamName)
        {
            var builtInValidators = new PropertyValidator<T, string>[]
            {
                new NotEmptyValidator<T, string>(),
                new LengthValidator<T>(3, 50)
            };

            builtInValidators.ForEach(v => v.IsValid(context, teamName));
        }

        private static void ValidateWithCustomValidators(ValidationContext<T> context, string teamName)
        {
            if (string.IsNullOrEmpty(teamName))
                return;

            if (!Regex.IsMatch(teamName, @"^[a-zA-Z\-\.,_ ]+$"))
                context.AddFailure($"'{context.PropertyName}' may only contain letters, hyphens, dots, commas, underscores and spaces.");
            if (!char.IsLetter(teamName[0]))
                context.AddFailure($"'{context.PropertyName}' must begin with a letter.");
            if (!char.IsLetter(teamName[^1]))
                context.AddFailure($"'{context.PropertyName}' must end with a letter.");
            if (teamName.Contains("  "))
                context.AddFailure($"'{context.PropertyName}' cannot contain two consecutive spaces.");
            if (Regex.IsMatch(teamName, @"[\-\.,_]{2}"))
                context.AddFailure($"'{context.PropertyName}' cannot contain two consecutive symbols.");
        }
    }
}
