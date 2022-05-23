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

        public IdentityService(JwtSettings jwtSettings, IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<IdentityResult> RegisterAsync(Username username, Password password, Device device, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(username, cancellationToken);
            if (existingUser is not null)
                throw new IdentityException(new[] { "User with this username already exists." });

            var createdUser = await _userRepository.CreateAsync(username, password);
            if (createdUser is null)
                throw new IdentityException(new[] { $"Failed to create user {username}" });

            var token = new Token();
            var refreshToken = new RefreshToken();
            return new IdentityResult(token, refreshToken);
        }

        public async Task<IdentityResult> LoginAsync(Username username, Password password, Device device, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> RefreshAsync(Token token, RefreshToken refreshToken, Device device, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
