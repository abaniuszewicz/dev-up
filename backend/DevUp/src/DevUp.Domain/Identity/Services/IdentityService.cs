using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.ValueObjects;

using DevUp.Domain.Identity.Services.Enums;
using DevUp.Domain.Identity.Services.Exceptions;

namespace DevUp.Domain.Identity.Services
{
    internal class IdentityService : IIdentityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public IdentityService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IPasswordService passwordService,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<IdentityResult> RegisterAsync(Username username, Password password, Device device, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(username, cancellationToken);
            if (existingUser is not null)
                throw new UsernameTakenException(username);

            var passwordHash = await _passwordService.HashAsync(password, cancellationToken);
            var createdUser = await _userRepository.CreateAsync(username, passwordHash, cancellationToken);
            (var token, var refreshToken) = await _tokenService.CreateAsync(createdUser, device, cancellationToken);
            return new IdentityResult(token, refreshToken);
        }

        public async Task<IdentityResult> LoginAsync(Username username, Password password, Device device, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
            if (user is null)
                throw new UsernameNotFoundException(username);

            var passwordHash = await _userRepository.GetPasswordHashAsync(user, cancellationToken);
            if (passwordHash is null)
                throw new PasswordHashNotFoundException(username);

            var verificationResult = await _passwordService.VerifyAsync(password, passwordHash, cancellationToken);
            if (verificationResult == PasswordVerifyResult.Failed)
                throw new InvalidPasswordException(username);

            (var token, var refreshToken) = await _tokenService.CreateAsync(user, device, cancellationToken);
            return new IdentityResult(token, refreshToken);
        }

        public async Task<IdentityResult> RefreshAsync(Token token, RefreshToken refreshToken, Device device, CancellationToken cancellationToken)
        {
            var tokenInfo = await _tokenService.DescribeAsync(token, cancellationToken);
            var refreshTokenInfo = await _tokenService.DescribeAsync(refreshToken, cancellationToken);

            await _tokenService.ValidateAsync(tokenInfo, refreshTokenInfo, device, cancellationToken);
            await _refreshTokenRepository.MarkAsUsedAsync(refreshTokenInfo, cancellationToken);
            var user = await _userRepository.GetByIdAsync(tokenInfo.UserId, cancellationToken);

            (var newToken, var newRefreshToken) = await _tokenService.CreateAsync(user, device, cancellationToken);
            return new IdentityResult(newToken, newRefreshToken);
        }
    }
}
