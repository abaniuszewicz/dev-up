using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevUp.Domain.Identity;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.Results;
using DevUp.Infrastructure.Identity.Stores;
using DevUp.Infrastructure.JwtIdentity.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DevUp.Infrastructure.Identity
{
    internal class JwtIdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;

        public JwtIdentityService(UserManager<User> userManager, JwtSettings jwtSettings)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _jwtSettings = jwtSettings;
        }

        public async Task<RegistrationResult> RegisterAsync(string username, string password)
        {
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser is not null)
                throw new RegistrationFailedException(new[] { "User with this username already exists." });

            var user = new User() { Username = username };
            var createdUser = await _userManager.CreateAsync(user, password);
            if (!createdUser.Succeeded)
                throw new RegistrationFailedException(createdUser.Errors.Select(e => e.Description));

            var token = GenerateJwtToken(user);
            return new JwtRegistrationResult() { Token = token };
        }

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                throw new LoginFailedException(new[] { "User with this username does not exist." });

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
                throw new LoginFailedException(new[] { "Invalid password." });

            var token = GenerateJwtToken(user);
            return new JwtLoginResult() { Token = token };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(GetClaims(user.Username)),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtSettings.Secret), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static IEnumerable<Claim> GetClaims(string username)
        {
            yield return new Claim(JwtRegisteredClaimNames.Sub, username);
            yield return new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
        }
    }
}
