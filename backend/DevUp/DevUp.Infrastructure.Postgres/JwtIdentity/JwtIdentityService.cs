using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.Services;
using DevUp.Infrastructure.Postgres.JwtIdentity.Dtos;
using DevUp.Infrastructure.Postgres.JwtIdentity.Results;
using DevUp.Infrastructure.Postgres.JwtIdentity.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DevUp.Infrastructure.Postgres.JwtIdentity
{
    internal class JwtIdentityService : IIdentityService
    {
        private const string SecurityAlghoritm = SecurityAlgorithms.HmacSha256;

        private readonly UserManager<UserDto> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IRefreshTokenStore _refreshTokenStore;

        public JwtIdentityService(UserManager<UserDto> userManager, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters, IRefreshTokenStore refreshTokenStore)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
            _tokenValidationParameters = tokenValidationParameters ?? throw new ArgumentNullException(nameof(tokenValidationParameters));
            _refreshTokenStore = refreshTokenStore ?? throw new ArgumentNullException(nameof(refreshTokenStore));
        }

        public async Task<IRegistrationResult> RegisterAsync(string username, string password, Device device)
        {
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser is not null)
                throw new IdentityException(new[] { "User with this username already exists." });

            var user = new UserDto() { UserName = username };
            var createdUser = await _userManager.CreateAsync(user, password);
            if (!createdUser.Succeeded)
                throw new IdentityException(createdUser.Errors.Select(e => e.Description));

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = GenerateToken(tokenHandler, user);
            var refreshTokenDto = GenerateRefreshTokenDto(token, user, device);
            var createdRefreshToken = await _refreshTokenStore.CreateAsync(refreshTokenDto, CancellationToken.None);
            if (!createdRefreshToken.Succeeded)
                throw new IdentityException(createdRefreshToken.Errors.Select(e => e.Description));

            return new JwtRegistrationResult() { Token = tokenHandler.WriteToken(token), RefreshToken = refreshTokenDto.Token };
        }

        public async Task<ILoginResult> LoginAsync(string username, string password, Device device)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                throw new IdentityException(new[] { "User with this username does not exist." });

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
                throw new IdentityException(new[] { "Invalid password." });

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = GenerateToken(tokenHandler, user);
            var refreshTokenDto = GenerateRefreshTokenDto(token, user, device);
            var createdRefreshToken = await _refreshTokenStore.CreateAsync(refreshTokenDto, CancellationToken.None);
            if (!createdRefreshToken.Succeeded)
                throw new IdentityException(createdRefreshToken.Errors.Select(e => e.Description));

            return new JwtLoginResult() { Token = tokenHandler.WriteToken(token), RefreshToken = refreshTokenDto.Token };
        }

        public async Task<IRefreshResult> RefreshAsync(string token, string refreshToken, Device device)
        {
            var principal = GetPrincipal(token);
            if (principal is null)
                throw new IdentityException(new[] { "Token is invalid." });

            var refreshTokenDto = await _refreshTokenStore.GetAsync(refreshToken, CancellationToken.None);
            if (refreshTokenDto is null)
                throw new IdentityException(new[] { "Refresh token does not exist." });

            var username = principal.FindFirstValue("username");
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                throw new IdentityException(new[] { "Token did not contain username of an existing user." });

            ValidateTokens(user, principal, refreshTokenDto, device);

            var markedAsUsed = await _refreshTokenStore.MarkAsUsed(refreshTokenDto, CancellationToken.None);
            if (!markedAsUsed.Succeeded)
                throw new IdentityException(markedAsUsed.Errors.Select(e => e.Description));

            var tokenHandler = new JwtSecurityTokenHandler();
            var newToken = GenerateToken(tokenHandler, user);
            var newRefreshTokenDto = GenerateRefreshTokenDto(newToken, user, device);
            var createdRefreshToken = await _refreshTokenStore.CreateAsync(newRefreshTokenDto, CancellationToken.None);
            if (!createdRefreshToken.Succeeded)
                throw new IdentityException(createdRefreshToken.Errors.Select(e => e.Description));

            return new JwtRefreshResult() { Token = tokenHandler.WriteToken(newToken), RefreshToken = newRefreshTokenDto.Token };
        }

        private SecurityToken GenerateToken(JwtSecurityTokenHandler tokenHandler, UserDto user)
        {
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(GetClaims(user.UserName)),
                Expires = DateTime.UtcNow.AddMilliseconds(_jwtSettings.JwtExpiryMs),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtSettings.Secret), SecurityAlghoritm)
            };

            return tokenHandler.CreateToken(tokenDescriptor);
        }

        private RefreshTokenDto GenerateRefreshTokenDto(SecurityToken token, UserDto user, Device device)
        {
            var now = DateTime.UtcNow;
            return new RefreshTokenDto()
            {
                Token = GenerateRefreshToken(),
                Jti = token.Id,
                UserId = user.Id,
                CreationDate = now,
                ExpiryDate = now.AddMilliseconds(_jwtSettings.JwtRefreshExpiryMs),
                Device = new DeviceDto() 
                {
                    Id = device.Id.Id,
                    Name = device.Name
                },
                Used = false,
                Invalidated = false,
            };
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private static IEnumerable<Claim> GetClaims(string username)
        {
            yield return new Claim("username", username);
            yield return new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
        }

        private ClaimsPrincipal? GetPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (validatedToken is not JwtSecurityToken jwtSecurityToken)
                    return null;
                if (!SecurityAlghoritm.Equals(jwtSecurityToken.Header.Alg, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private void ValidateTokens(UserDto user, ClaimsPrincipal principal, RefreshTokenDto refreshToken, Device device)
        {
            static bool Equals(string s1, string s2) => s1 is not null && string.Equals(s1, s2, StringComparison.InvariantCulture);

            var expiryDateUnix = long.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Exp));
            var expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiryDateUnix).UtcDateTime;
            if (expiryDate > DateTime.UtcNow)
                throw new IdentityException(new[] { "Token has not expired yet." });

            if (refreshToken.ExpiryDate < DateTime.UtcNow)
                throw new IdentityException(new[] { "Refresh token has expired." });

            if (refreshToken.Invalidated)
                throw new IdentityException(new[] { "Refresh token has been invalidated." });

            if (refreshToken.Used)
                throw new IdentityException(new[] { "Refresh token has been used already." });

            var jti = principal.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            if (!Equals(jti, refreshToken.Jti))
                throw new IdentityException(new[] { "Refresh token does not belong to this token." });

            if (refreshToken.UserId != user.Id)
                throw new IdentityException(new[] { "Refresh token does not belong to this user." });

            if (!Equals(refreshToken.Device.Id, device.Id.Id))
                throw new IdentityException(new[] { "Refresh token does not belong to this device." });
        }
    }
}
