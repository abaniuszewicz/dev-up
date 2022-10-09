using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.Services.Exceptions;
using DevUp.Domain.Identity.Setup;
using DevUp.Domain.Identity.ValueObjects;
using DevUp.Infrastructure.Identity.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace DevUp.Domain.Tests.Integration.Identity.Services
{
    public class TokenServiceTests_RefreshAsync
    {
        private readonly Faker<User> _userFaker = new Faker<User>()
            .CustomInstantiator(f => new(new(f.Random.Guid()), new(f.Person.UserName)));
        private readonly Faker<Device> _deviceFaker = new Faker<Device>()
            .CustomInstantiator(f => new(new(f.Random.Guid()), $"{f.Person.FirstName} PC"));
        private readonly Faker<Username> _usernameFaker = new Faker<Username>()
            .CustomInstantiator(f => new(f.Internet.UserName()));
        private readonly Faker<PasswordHash> _passwordHashFaker = new Faker<PasswordHash>()
            .CustomInstantiator(f => new(f.Random.Hash()));

        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IOptions<AuthenticationOptions> _authenticationOptions;

        private readonly ITokenService _tokenService;

        public TokenServiceTests_RefreshAsync()
        {
            var faker = new Faker();
            _userRepository = new InMemoryUserRepository();
            _refreshTokenRepository = new InMemoryRefreshTokenRepository();
            _deviceRepository = new InMemoryDeviceRepository();
            _dateTimeProvider = new UtcDateTimeProvider();
            var expiry = TimeSpan.FromSeconds(1) + faker.Date.Timespan(maxSpan: TimeSpan.FromSeconds(2)); // random 1÷3 seconds
            _authenticationOptions = Options.Create(new AuthenticationOptions()
            {
                TokenExpiry = expiry,
                RefreshTokenExpiry = 5 * expiry,
                SigningKey = faker.Random.String2(32),
                Algorithm = faker.PickRandom(SecurityAlgorithms.HmacSha256, SecurityAlgorithms.HmacSha512)
            });

            _tokenService = new TokenService(_userRepository, _refreshTokenRepository, _deviceRepository, _dateTimeProvider, _authenticationOptions);
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenDoesNotExistInRepository_ThrowsRefreshTokenInfoIdNotFoundException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();
            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            var nonExistingIdPair = new TokenPair(tokenPair.Token, new RefreshToken("this-value-doesnt-exist-in-repository"));

            await Task.Delay(_authenticationOptions.Value.TokenExpiry);
            var refresh = async () => await _tokenService.RefreshAsync(nonExistingIdPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<RefreshTokenInfoIdNotFoundException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenWasIssuedForDifferentToken_ThrowsTokenMismatchException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            var rti = await _refreshTokenRepository.GetByIdAsync(new(tokenPair.RefreshToken), CancellationToken.None);
            var rtiWithDifferentJti = new RefreshTokenInfo(rti.Id, "different_token_jti", rti.UserId, rti.DeviceId, rti.Lifespan);
            await _refreshTokenRepository.UpdateAsync(rtiWithDifferentJti, CancellationToken.None);

            await Task.Delay(_authenticationOptions.Value.TokenExpiry);
            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<TokenMismatchException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenWasAlreadyInvalidated_ThrowsRefreshTokenInvalidatedException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            var rti = await _refreshTokenRepository.GetByIdAsync(new(tokenPair.RefreshToken), CancellationToken.None);
            rti.Invalidated = true;
            await _refreshTokenRepository.UpdateAsync(rti, CancellationToken.None);

            await Task.Delay(_authenticationOptions.Value.TokenExpiry);
            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<RefreshTokenInvalidatedException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenWasAlreadyUsed_ThrowsRefreshTokenUsedException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            var rti = await _refreshTokenRepository.GetByIdAsync(new(tokenPair.RefreshToken), CancellationToken.None);
            rti.Used = true;
            await _refreshTokenRepository.UpdateAsync(rti, CancellationToken.None);

            await Task.Delay(_authenticationOptions.Value.TokenExpiry);
            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<RefreshTokenUsedException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenTokenHasNotExpiredYet_ThrowsTokenNotExpiredException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);

            await Task.Delay(_authenticationOptions.Value.TokenExpiry.Milliseconds);
            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<TokenNotExpiredException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenHasExpired_ThrowsRefreshTokenExpiredException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);

            await Task.Delay(_authenticationOptions.Value.RefreshTokenExpiry);
            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<RefreshTokenExpiredException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenTokenPointsToNonExistingUser_ThrowsUserIdNotFoundException()
        {
            var user = _userFaker.Generate();
            var device = await CreateDevice();

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);

            await Task.Delay(_authenticationOptions.Value.TokenExpiry);
            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<UserIdNotFoundException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenPointsToDifferentUserThanToken_ThrowsUserIdMismatchException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();
            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            var rti = await _refreshTokenRepository.GetByIdAsync(new(tokenPair.RefreshToken), CancellationToken.None);
            var rtiWithDifferentUserId = new RefreshTokenInfo(rti.Id, rti.Jti, new UserId(), rti.DeviceId, rti.Lifespan);
            await _refreshTokenRepository.UpdateAsync(rtiWithDifferentUserId, CancellationToken.None);

            await Task.Delay(_authenticationOptions.Value.TokenExpiry);
            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<UserIdMismatchException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenTokenPointsToNonExistingDevice_ThrowsDeviceIdNotFoundException()
        {
            var user = await CreateUser();
            var device = _deviceFaker.Generate();
            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);

            await Task.Delay(_authenticationOptions.Value.TokenExpiry);
            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<DeviceIdNotFoundException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenPointsToDifferentDeviceThanToken_ThrowsDeviceIdMismatchException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();
            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            var rti = await _refreshTokenRepository.GetByIdAsync(new(tokenPair.RefreshToken), CancellationToken.None);
            var rtiWithDifferentDeviceId = new RefreshTokenInfo(rti.Id, rti.Jti, rti.UserId, new DeviceId(Guid.NewGuid()), rti.Lifespan);
            await _refreshTokenRepository.UpdateAsync(rtiWithDifferentDeviceId, CancellationToken.None);

            await Task.Delay(_authenticationOptions.Value.TokenExpiry);
            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<DeviceIdMismatchException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenTryingToRefreshFromDifferentDeviceThanTokenWasIssuedFor_ThrowsDeviceIdMismatchException()
        {
            var user = await CreateUser();
            var device = await CreateDevice();
            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);

            await Task.Delay(_authenticationOptions.Value.TokenExpiry);
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
    }
}
