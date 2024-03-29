﻿using System;
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
using DevUp.Domain.Identity.Services.Exceptions;
using DevUp.Domain.Identity.Setup;
using DevUp.Domain.Identity.ValueObjects;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DevUp.Domain.Identity.Services
{
    internal class TokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly AuthenticationOptions _authenticationOptions;

        public TokenService(IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IDeviceRepository deviceRepository,
            IDateTimeProvider dateTimeProvider, 
            IOptions<AuthenticationOptions> authenticationOptions)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _deviceRepository = deviceRepository;
            _dateTimeProvider = dateTimeProvider;
            _authenticationOptions = authenticationOptions.Value;
        }

        public async Task<TokenPair> CreateAsync(UserId userId, DeviceId deviceId, CancellationToken cancellationToken)
        {
            var now = _dateTimeProvider.Now;

            var tokenHandler = new JwtSecurityTokenHandler();
            var jti = Guid.NewGuid().ToString();
            var tokenLifespan = new DateTimeRange(now, now.Add(_authenticationOptions.TokenExpiry));
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, jti),
                    new Claim(JwtCustomClaimNames.DeviceId, deviceId.ToString())
                }),
                NotBefore = tokenLifespan.Start,
                Expires = tokenLifespan.End,
                SigningCredentials = _authenticationOptions.GetSigningCredentials()
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(securityToken);
            var token = new Token(jwt);

            var refreshToken = new RefreshToken();
            var refreshTokenInfoId = new RefreshTokenInfoId(refreshToken);
            var refreshTokenLifespan = new DateTimeRange(now, now.Add(_authenticationOptions.RefreshTokenExpiry));
            var refreshTokenInfo = new RefreshTokenInfo(refreshTokenInfoId, jti, userId, deviceId, refreshTokenLifespan);

            await _refreshTokenRepository.AddAsync(refreshTokenInfo, cancellationToken);
            return new TokenPair(token, refreshToken);
        }

        public async Task<TokenPair> RefreshAsync(TokenPair oldTokenPair, Device device, CancellationToken cancellationToken)
        {
            var oldTokenInfo = await DescribeAsync(oldTokenPair.Token, cancellationToken);
            var oldRefreshTokenInfoId = new RefreshTokenInfoId(oldTokenPair.RefreshToken);
            var oldRefreshTokenInfo = await _refreshTokenRepository.GetByIdAsync(oldRefreshTokenInfoId, cancellationToken)
                ?? throw new RefreshTokenInfoIdNotFoundException(oldRefreshTokenInfoId);

            try
            {
                await ValidateAsync(oldTokenInfo, oldRefreshTokenInfo, device, cancellationToken);
            }
            catch (Exception exception) when (exception is IIdentityException)
            {
                await RevokeAsync(oldTokenPair.RefreshToken, cancellationToken);
                throw;
            }

            await _refreshTokenRepository.MarkAsUsedAsync(oldRefreshTokenInfo, cancellationToken);
            var newTokenPair = await CreateAsync(oldTokenInfo.UserId, oldTokenInfo.DeviceId, cancellationToken);
            var newRefreshTokenInfoId = new RefreshTokenInfoId(newTokenPair.RefreshToken);
            await _refreshTokenRepository.ChainAsync(oldRefreshTokenInfoId, newRefreshTokenInfoId, cancellationToken);

            return newTokenPair;
        }

        public async Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            var id = new RefreshTokenInfoId(refreshToken);
            var refreshTokenInfo = await _refreshTokenRepository.GetByIdAsync(id, cancellationToken);
            if (refreshTokenInfo is null)
                throw new RefreshTokenInfoIdNotFoundException(id);

            await _refreshTokenRepository.InvalidateChainAsync(refreshTokenInfo, cancellationToken);
        }

        private async Task<TokenInfo> DescribeAsync(Token token, CancellationToken cancellationToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var result = await tokenHandler.ValidateTokenAsync(token.Value, _authenticationOptions.GetTokenValidationParameters());
            if (!result.IsValid)
                throw new TokenDescriptionException(token, result.Exception);

            var jwtSecurityToken = (JwtSecurityToken)result.SecurityToken;
            var jti = jwtSecurityToken.Id;
            var lifespan = new DateTimeRange(jwtSecurityToken.ValidFrom, jwtSecurityToken.ValidTo);

            var userIdClaim = jwtSecurityToken.Subject;
            var userGuid = Guid.Parse(userIdClaim);
            var userId = new UserId(userGuid);

            var deviceIdClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == JwtCustomClaimNames.DeviceId)?.Value;
            var deviceGuid = Guid.Parse(deviceIdClaim);
            var deviceId = new DeviceId(deviceGuid);

            return new TokenInfo(token, jti, userId, deviceId, lifespan);
        }

        private async Task ValidateAsync(TokenInfo token, RefreshTokenInfo refreshToken, Device currentDevice, CancellationToken cancellationToken)
        {
            // should tokens be validated
            if (!refreshToken.BelongsTo(token))
                throw new TokenMismatchException(token, refreshToken);
            if (refreshToken.Invalidated)
                throw new RefreshTokenInvalidatedException(refreshToken.Id);
            if (refreshToken.Used)
                throw new RefreshTokenUsedException(refreshToken.Id);

            // have tokens expired
            if (token.IsActive(_dateTimeProvider))
                throw new TokenNotExpiredException(token, _dateTimeProvider);
            if (!refreshToken.IsActive(_dateTimeProvider))
                throw new RefreshTokenExpiredException(refreshToken, _dateTimeProvider);

            // do tokens belong to a valid user
            var user = await _userRepository.GetByIdAsync(token.UserId, cancellationToken);
            if (user is null)
                throw new UserIdNotFoundException(token.UserId);
            if (!refreshToken.BelongsTo(user))
                throw new UserIdMismatchException(token.UserId, refreshToken.UserId);

            // do tokens belong to a valid device
            var device = await _deviceRepository.GetByIdAsync(token.DeviceId, cancellationToken);
            if (device is null)
                throw new DeviceIdNotFoundException(token.DeviceId);
            if (new[] { device, currentDevice }.Any(d => !refreshToken.BelongsTo(d)))
                throw new DeviceIdMismatchException(token.DeviceId, refreshToken.DeviceId, currentDevice.Id);
        }

    }
}
