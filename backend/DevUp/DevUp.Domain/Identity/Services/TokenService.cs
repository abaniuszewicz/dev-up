using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevUp.Common;
using DevUp.Domain.Identity.Creation;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services
{
    internal class TokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly JwtSettings _jwtSettings;

        public TokenService(IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IDeviceRepository deviceRepository,
            IDateTimeProvider dateTimeProvider, 
            JwtSettings jwtSettings)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _deviceRepository = deviceRepository;
            _dateTimeProvider = dateTimeProvider;
            _jwtSettings = jwtSettings;
        }

        public async Task<(Token, RefreshToken)> CreateAsync(User user, Device device, CancellationToken cancellationToken)
        {
            var tokenInfo = new TokenBuilder().ForUser(user).ForDevice(device).WithSettings(_jwtSettings)
                .WithTimeProvider(_dateTimeProvider)
                .Build();
            var refreshTokenInfo = new RefreshTokenBuilder().FromTokenInfo(tokenInfo).ForUser(user)
                .ForDevice(device).WithSettings(_jwtSettings).WithTimeProvider(_dateTimeProvider)
                .Build();

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
            if (!TokenBuilder.SecurityAlghoritm.Equals(jwtSecurityToken.Header.Alg, StringComparison.InvariantCulture))
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
                errors.Add("Token did not contain id of an existing user");
            if (user is not null && !refreshToken.BelongsTo(user))
                errors.Add("Refresh token does not belong to this user");

            if (!token.IsActive(_dateTimeProvider))
                errors.Add("Token is no longer active");

            if (!refreshToken.IsActive(_dateTimeProvider))
                errors.Add("Refresh token is no longer active");
            if (refreshToken.Invalidated)
                errors.Add("Refresh token has been invalidated");
            if (refreshToken.Used)
                errors.Add("Refresh token has been already used");
            if (!refreshToken.BelongsTo(token))
                errors.Add("Refresh token does not belong to this token");
            if (!refreshToken.BelongsTo(device))
                errors.Add("Refresh token does not belong to this device");

            if (errors.Any())
                throw new IdentityException(errors);
        }
    }
}
