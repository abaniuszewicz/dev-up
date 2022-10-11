using System.Threading;
using System.Threading.Tasks;
using Bogus;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.Services.Exceptions;
using DevUp.Domain.Identity.ValueObjects;
using DevUp.Infrastructure.Identity.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace DevUp.Domain.Tests.Integration.Identity.Services
{
    public class TokenServiceTests_RefreshAsync : IClassFixture<TokenServiceFactory>
    {
        private readonly Faker<User> _userFaker = new Faker<User>().CustomInstantiator(f => new(new(f.Random.Guid()), new(f.Person.UserName)));
        private readonly Faker<Device> _deviceFaker = new Faker<Device>().CustomInstantiator(f => new(new(f.Random.Guid()), $"{f.Person.FirstName} PC"));
        private readonly Faker<Username> _usernameFaker = new Faker<Username>().CustomInstantiator(f => new(f.Internet.UserName()));
        private readonly Faker<PasswordHash> _passwordHashFaker = new Faker<PasswordHash>().CustomInstantiator(f => new(f.Random.Hash()));

        private readonly TokenServiceFactory _tokenServiceFactory;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public TokenServiceTests_RefreshAsync(TokenServiceFactory tokenServiceFactory)
        {
            _tokenServiceFactory = tokenServiceFactory;
            _tokenService = tokenServiceFactory.Create<ITokenService>();
            _userRepository = tokenServiceFactory.Create<IUserRepository>();
            _deviceRepository = tokenServiceFactory.Create<IDeviceRepository>();
            _refreshTokenRepository = tokenServiceFactory.Create<IRefreshTokenRepository>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenDoesNotExistInRepository_ThrowsRefreshTokenInfoIdNotFoundException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();
            var tokenPair = await CreateTokenPair(user, device);

            var nonExistingIdPair = new TokenPair(tokenPair.Token, new RefreshToken("this-value-doesnt-exist-in-repository"));
            var refresh = async () => await _tokenService.RefreshAsync(nonExistingIdPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<RefreshTokenInfoIdNotFoundException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenWasIssuedForDifferentToken_ThrowsTokenMismatchException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();
            var tokenPair = await CreateTokenPair(user, device);
            var differentTokenPair = await CreateTokenPair(user, device);

            var rti = await _refreshTokenRepository.GetByIdAsync(new(tokenPair.RefreshToken), CancellationToken.None);
            var differentRti = await _refreshTokenRepository.GetByIdAsync(new(differentTokenPair.RefreshToken), CancellationToken.None);
            var rtiWithDifferentJti = new RefreshTokenInfo(rti.Id, differentRti.Jti, rti.UserId, rti.DeviceId, rti.Lifespan);
            await _refreshTokenRepository.UpdateAsync(rtiWithDifferentJti, CancellationToken.None);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<TokenMismatchException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenWasAlreadyInvalidated_ThrowsRefreshTokenInvalidatedException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();
            var tokenPair = await CreateTokenPair(user, device);

            var rti = await _refreshTokenRepository.GetByIdAsync(new(tokenPair.RefreshToken), CancellationToken.None);
            rti.Invalidated = true;
            await _refreshTokenRepository.UpdateAsync(rti, CancellationToken.None);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<RefreshTokenInvalidatedException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenWasAlreadyUsed_ThrowsRefreshTokenUsedException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();
            var tokenPair = await CreateTokenPair(user, device);

            var rti = await _refreshTokenRepository.GetByIdAsync(new(tokenPair.RefreshToken), CancellationToken.None);
            rti.Used = true;
            await _refreshTokenRepository.UpdateAsync(rti, CancellationToken.None);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<RefreshTokenUsedException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenTokenHasNotExpiredYet_ThrowsTokenNotExpiredException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();
            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<TokenNotExpiredException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenHasExpired_ThrowsRefreshTokenExpiredException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();
            var tokenPair = await CreateTokenPair(user, device);

            await Task.Delay(_tokenServiceFactory.AuthenticationOptions.RefreshTokenExpiry);
            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<RefreshTokenExpiredException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenTokenPointsToNonExistingUser_ThrowsUserIdNotFoundException()
        {
            // this cannot be achieved through postgres repository due to FK constraint that enforces data integrity
            var userRepository = new InMemoryUserRepository();
            var refreshTokenRepository = new InMemoryRefreshTokenRepository();
            var deviceRepository = new InMemoryDeviceRepository();
            var dateTimeProvider = _tokenServiceFactory.Create<IDateTimeProvider>();
            var authOptions = Options.Create(_tokenServiceFactory.AuthenticationOptions);
            var tokenService = new TokenService(userRepository, refreshTokenRepository, deviceRepository, dateTimeProvider, authOptions);

            var user = _userFaker.Generate();
            var device = _deviceFaker.Generate();
            await deviceRepository.AddAsync(device, CancellationToken.None);
            var tokenPair = await tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);

            await Task.Delay(_tokenServiceFactory.AuthenticationOptions.TokenExpiry);
            var refresh = async () => await tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<UserIdNotFoundException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenPointsToDifferentUserThanToken_ThrowsUserIdMismatchException()
        {
            var user = await CreateUser();
            var differentUser = await CreateUser();
            var device = await CreateDevice();
            var tokenPair = await CreateTokenPair(user, device);
            var rti = await _refreshTokenRepository.GetByIdAsync(new(tokenPair.RefreshToken), CancellationToken.None);
            var rtiWithDifferentUserId = new RefreshTokenInfo(rti.Id, rti.Jti, differentUser.Id, rti.DeviceId, rti.Lifespan);
            await _refreshTokenRepository.UpdateAsync(rtiWithDifferentUserId, CancellationToken.None);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<UserIdMismatchException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenTokenPointsToNonExistingDevice_ThrowsDeviceIdNotFoundException()
        {
            // this cannot be achieved through postgres repository due to FK constraint that enforces data integrity
            var userRepository = new InMemoryUserRepository();
            var refreshTokenRepository = new InMemoryRefreshTokenRepository();
            var deviceRepository = new InMemoryDeviceRepository();
            var dateTimeProvider = _tokenServiceFactory.Create<IDateTimeProvider>();
            var authOptions = Options.Create(_tokenServiceFactory.AuthenticationOptions);
            var tokenService = new TokenService(userRepository, refreshTokenRepository, deviceRepository, dateTimeProvider, authOptions);

            var user = await userRepository.CreateAsync(_usernameFaker.Generate(), _passwordHashFaker.Generate(), CancellationToken.None);
            var device = _deviceFaker.Generate();
            var tokenPair = await tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);

            await Task.Delay(_tokenServiceFactory.AuthenticationOptions.TokenExpiry);
            var refresh = async () => await tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<DeviceIdNotFoundException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenPointsToDifferentDeviceThanToken_ThrowsDeviceIdMismatchException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();
            var differentDevice = await CreateDevice();
            var tokenPair = await CreateTokenPair(user, device);
            var rti = await _refreshTokenRepository.GetByIdAsync(new(tokenPair.RefreshToken), CancellationToken.None);
            var rtiWithDifferentDeviceId = new RefreshTokenInfo(rti.Id, rti.Jti, rti.UserId, differentDevice.Id, rti.Lifespan);
            await _refreshTokenRepository.UpdateAsync(rtiWithDifferentDeviceId, CancellationToken.None);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<DeviceIdMismatchException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenTryingToRefreshFromDifferentDeviceThanTokenWasIssuedFor_ThrowsDeviceIdMismatchException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();
            var tokenPair = await CreateTokenPair(user, device);
            
            var differentDevice = _deviceFaker.Generate();
            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, differentDevice, CancellationToken.None);

            await refresh.Should().ThrowAsync<DeviceIdMismatchException>();
        }

        private async Task<User> CreateUser()
        {
            var username = _usernameFaker.Generate();
            var passwordHash = _passwordHashFaker.Generate();
            return await _userRepository.CreateAsync(username, passwordHash, CancellationToken.None);
        }

        private async Task<Device> CreateDevice()
        {
            var device = _deviceFaker.Generate();
            await _deviceRepository.AddAsync(device, CancellationToken.None);
            return device;
        }

        private async Task<TokenPair> CreateTokenPair(User user, Device device)
        {
            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            await Task.Delay(_tokenServiceFactory.AuthenticationOptions.TokenExpiry);
            return tokenPair;
        }
    }
}
