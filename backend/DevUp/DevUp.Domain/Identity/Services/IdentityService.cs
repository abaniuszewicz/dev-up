using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services
{
    internal class IdentityService : IIdentityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public IdentityService(JwtSettings jwtSettings, IUserRepository userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
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
            var refreshToken = new RefreshToken();
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
            if (verificationResult == Enums.PasswordVerifyResult.Failed)
                throw new IdentityException(new[] { $"Invalid password" });

            var token = new Token();
            var refreshToken = new RefreshToken();
            return new IdentityResult(token, refreshToken);
        }

        public Task<IdentityResult> RefreshAsync(Token token, RefreshToken refreshToken, Device device, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
