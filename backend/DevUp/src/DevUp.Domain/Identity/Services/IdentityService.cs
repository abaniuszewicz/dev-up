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
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IDeviceRepository _deviceRepository;

        public IdentityService(
            IUserRepository userRepository,
            IDeviceRepository deviceRepository,
            IPasswordService passwordService,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _deviceRepository = deviceRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<TokenPair> RegisterAsync(Username username, Password password, Device device, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(username, cancellationToken);
            if (existingUser is not null)
                throw new UsernameTakenException(username);

            await _deviceRepository.AddAsync(device, cancellationToken);
            var passwordHash = await _passwordService.HashAsync(password, cancellationToken);
            var createdUser = await _userRepository.CreateAsync(username, passwordHash, cancellationToken);
            return await _tokenService.CreateAsync(createdUser.Id, device.Id, cancellationToken);
        }

        public async Task<TokenPair> LoginAsync(Username username, Password password, Device device, CancellationToken cancellationToken)
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

            return await _tokenService.CreateAsync(user.Id, device.Id, cancellationToken);
        }
    }
}
