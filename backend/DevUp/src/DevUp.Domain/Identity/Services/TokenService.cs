using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity.Entities;
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
        private const string SecurityAlghoritm = SecurityAlgorithms.HmacSha256;

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

        public async Task<(Token, RefreshToken)> CreateAsync(User user, Device device, CancellationToken cancellationToken)
        {
            var now = _dateTimeProvider.Now;

            var tokenHandler = new JwtSecurityTokenHandler();
            var jti = Guid.NewGuid().ToString();
            var tokenLifespan = new DateTimeRange(now, now.Add(_authenticationOptions.TokenExpiry));
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, jti),
                    new Claim(JwtCustomClaimNames.DeviceId, device.Id.ToString())
                }),
                NotBefore = tokenLifespan.Start,
                Expires = tokenLifespan.End,
                SigningCredentials = new SigningCredentials(_authenticationOptions.GetTokenValidationParameters().IssuerSigningKey, SecurityAlghoritm)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(securityToken);
            var token = new Token(jwt);
            var tokenInfo = new TokenInfo(token, jti, user.Id, device.Id, tokenLifespan);

            var refreshToken = new RefreshToken();
            var refreshTokenLifespan = new DateTimeRange(now, now.Add(_authenticationOptions.RefreshTokenExpiry));
            var refreshTokenInfo = new RefreshTokenInfo(refreshToken, jti, user.Id, device.Id, refreshTokenLifespan);

            await _deviceRepository.AddAsync(device, cancellationToken);
            await _refreshTokenRepository.AddAsync(refreshTokenInfo, cancellationToken);
            return (tokenInfo.Token, refreshTokenInfo.Id);
        }

        public async Task<TokenInfo> DescribeAsync(Token token, CancellationToken cancellationToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var result = await tokenHandler.ValidateTokenAsync(token.Value, _authenticationOptions.GetTokenValidationParameters());
            if (!result.IsValid)
                throw new TokenDescriptionException(result.Exception);

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

        public async Task<RefreshTokenInfo> DescribeAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            return await _refreshTokenRepository.GetByIdAsync(refreshToken, cancellationToken)
                ?? throw new RefreshTokenInfoNotFoundException();
        }

        public async Task ValidateAsync(TokenInfo token, RefreshTokenInfo refreshToken, Device currentDevice, CancellationToken cancellationToken)
        {
            // should tokens be validated
            if (!refreshToken.BelongsTo(token))
                throw new TokenMismatchException(token, refreshToken);
            if (refreshToken.Invalidated)
                throw new RefreshTokenInvalidatedException();
            if (refreshToken.Used)
                throw new RefreshTokenUsedException();

            // have tokens expired
            if (token.IsActive(_dateTimeProvider))
                throw new TokenStillActiveException(token.Lifespan, _dateTimeProvider);
            if (!refreshToken.IsActive(_dateTimeProvider))
                throw new RefreshTokenNotActiveException(refreshToken.Lifespan, _dateTimeProvider);

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
