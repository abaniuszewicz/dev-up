using System.Threading;
using System.Threading.Tasks;
using DevUp.Common;
using DevUp.Domain.Identity.Creation;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Enums;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services
{
    internal class IdentityService : IIdentityService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordService _passwordService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public IdentityService(JwtSettings jwtSettings, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IPasswordService passwordService, IDateTimeProvider dateTimeProvider)
        {
            _jwtSettings = jwtSettings;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordService = passwordService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IdentityResult> RegisterAsync(Username username, Password password, Device device, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(username, cancellationToken);
            if (existingUser is not null)
                throw new IdentityException(new[] { "User with this username already exists." });

            var passwordHash = await _passwordService.HashAsync(password, cancellationToken);
            var createdUser = await _userRepository.CreateAsync(username, passwordHash, cancellationToken);
            if (createdUser is null)
                throw new IdentityException(new[] { $"Failed to create user {username}" });

            var token = new Token();
            var refreshToken = new RefreshTokenBuilder().FromToken(token).ForUser(createdUser)
                .ForDevice(device).WithSettings(_jwtSettings).WithTimeProvider(_dateTimeProvider)
                .Build();
            return new IdentityResult(token, refreshToken);
        }

        public async Task<IdentityResult> LoginAsync(Username username, Password password, Device device, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
            if (user is null)
                throw new IdentityException(new[] { "User with this username does not exists." });

            var passwordHash = await _userRepository.GetPasswordHashAsync(user, cancellationToken);
            if (passwordHash is null)
                throw new IdentityException(new[] { $"Failed to retrieve password hash for user {username}" });

            var verificationResult = await _passwordService.VerifyAsync(password, passwordHash, cancellationToken);
            if (verificationResult == PasswordVerifyResult.Failed)
                throw new IdentityException(new[] { $"Invalid password" });

            var token = new Token();
            var refreshToken = new RefreshTokenBuilder().FromToken(token).ForUser(user)
                .ForDevice(device).WithSettings(_jwtSettings).WithTimeProvider(_dateTimeProvider)
                .Build();
            return new IdentityResult(token, refreshToken);
        }

        public async Task<IdentityResult> RefreshAsync(Token token, RefreshTokenId refreshTokenId, Device device, CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshTokenRepository.GetByIdAsync(refreshTokenId, cancellationToken);
            if (refreshToken is null)
                throw new IdentityException(new[] { "Refresh token does not exist." });

        }
    }
}
