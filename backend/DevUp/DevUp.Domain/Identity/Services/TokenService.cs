using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.ValueObjects;
using Microsoft.IdentityModel.Tokens;

using static DevUp.Domain.Identity.Exceptions.TokenValidationException;

namespace DevUp.Domain.Identity.Services
{
    internal class TokenService : ITokenService
    {
        private const string SecurityAlghoritm = SecurityAlgorithms.HmacSha256;

        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IJwtSettings _jwtSettings;

        public TokenService(IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IDeviceRepository deviceRepository,
            IDateTimeProvider dateTimeProvider, 
            IJwtSettings jwtSettings)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _deviceRepository = deviceRepository;
            _dateTimeProvider = dateTimeProvider;
            _jwtSettings = jwtSettings;
        }

        public async Task<(Token, RefreshToken)> CreateAsync(User user, Device device, CancellationToken cancellationToken)
        {
            var now = _dateTimeProvider.Now;

            var tokenHandler = new JwtSecurityTokenHandler();
            var jti = Guid.NewGuid().ToString();
            var tokenLifespan = new DateTimeRange(now, now.AddMilliseconds(_jwtSettings.JwtExpiryMs));
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("sub", user.Id.ToString()),
                    new Claim("jti", jti)
                }),
                NotBefore = tokenLifespan.Start,
                Expires = tokenLifespan.End,
                SigningCredentials = new SigningCredentials(_jwtSettings.TokenValidationParameters.IssuerSigningKey, SecurityAlghoritm)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(securityToken);
            var token = new Token(jwt);
            var tokenInfo = new TokenInfo(token, jti, user.Id, tokenLifespan);

            var refreshToken = new RefreshToken();
            var refreshTokenLifespan = new DateTimeRange(now, now.AddMilliseconds(_jwtSettings.JwtRefreshExpiryMs));
            var refreshTokenInfo = new RefreshTokenInfo(refreshToken, jti, user.Id, device.Id, refreshTokenLifespan);

            await _deviceRepository.AddAsync(device, cancellationToken);
            await _refreshTokenRepository.AddAsync(refreshTokenInfo, cancellationToken);
            return (tokenInfo.Token, refreshTokenInfo.Id);
        }

        public async Task<TokenInfo> DescribeAsync(Token token, CancellationToken cancellationToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var result = await tokenHandler.ValidateTokenAsync(token.Value, _jwtSettings.TokenValidationParameters);
            if (!result.IsValid)
                return null;
            
            if (result.SecurityToken is not JwtSecurityToken jwtSecurityToken)
                return null;
            if (!SecurityAlghoritm.Equals(jwtSecurityToken.Header.Alg, StringComparison.InvariantCulture))
                return null;
            if (!Guid.TryParse(jwtSecurityToken.Subject, out var userGuid))
                return null;

            var userId = new UserId(userGuid);
            var jti = jwtSecurityToken.Id;
            var lifespan = new DateTimeRange(jwtSecurityToken.ValidFrom, jwtSecurityToken.ValidTo);
            return new TokenInfo(token, jti, userId, lifespan);
        }

        public async Task<RefreshTokenInfo> DescribeAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            return await _refreshTokenRepository.GetByIdAsync(refreshToken, cancellationToken);
        }

        public async Task ValidateAsync(TokenInfo token, RefreshTokenInfo refreshToken, Device device, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            var user = await _userRepository.GetByIdAsync(token.UserId, cancellationToken);
            if (user is null)
                errors.Add(TokenInvalidUserIdMessage);
            if (user is not null && !refreshToken.BelongsTo(user))
                errors.Add(RefreshTokenWrongUserMessage);

            if (!token.IsActive(_dateTimeProvider))
                errors.Add(TokenNotActiveMessage);

            if (!refreshToken.IsActive(_dateTimeProvider))
                errors.Add(RefreshTokenNotActiveMessage);
            if (refreshToken.Invalidated)
                errors.Add(RefreshTokenInvalidatedMessage);
            if (refreshToken.Used)
                errors.Add(RefreshTokenUsedMessage);
            if (!refreshToken.BelongsTo(token))
                errors.Add(RefreshTokenWrongTokenMessage);
            if (!refreshToken.BelongsTo(device))
                errors.Add(RefreshTokenWrongDeviceMessage);

            if (errors.Any())
                throw new IdentityException(errors);
        }
    }
}
