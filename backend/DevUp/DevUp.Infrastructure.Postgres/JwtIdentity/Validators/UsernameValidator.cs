using DevUp.Infrastructure.Postgres.JwtIdentity.Dtos;
using Microsoft.AspNetCore.Identity;

namespace DevUp.Infrastructure.Postgres.JwtIdentity.Validators
{
    internal class UsernameValidator : IUserValidator<UserDto>
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<UserDto> manager, UserDto user)
        {
            if (manager is null)
                throw new ArgumentNullException(nameof(manager));
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            var username = await manager.GetUserNameAsync(user) ?? string.Empty;
            var errors = new List<IdentityError>();

            if (username.Length < 6 || username.Length > 30)
                errors.Add(new() { Description = "Username must be 6-30 characters long." });
            if (username.Any(c => !IsValidCharacter(c)))
                errors.Add(new() { Description = "Username may only contain alphanumeric characters or hyphens." });
            if (username.StartsWith('-') || username.EndsWith('-'))
                errors.Add(new() { Description = "Username cannot begin or end with a hyphen." });

            return errors.Any() 
                ? IdentityResult.Failed(errors.ToArray())
                : IdentityResult.Success;
        }

        private static bool IsValidCharacter(char c)
        {
            return c switch
            {
                >= 'a' and <= 'z' => true,
                '-' => true,
                _ => false
            };
        }
    }
}
