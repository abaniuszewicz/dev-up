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
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Xunit;

namespace DevUp.Domain.Tests.Integration.Identity.Services
{
    public class TokenServiceTests_RefreshAsync
    {
        private readonly Faker _faker;
        private readonly Faker<User> _userFaker = new Faker<User>()
            .CustomInstantiator(f => new(new(f.Random.Guid()), new(f.Person.UserName)));
        private readonly Faker<Device> _deviceFaker = new Faker<Device>()
            .CustomInstantiator(f => new(new(f.Random.Guid()), $"{f.Person.FirstName} PC"));

        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IOptions<AuthenticationOptions> _authenticationOptions;

        private readonly ITokenService _tokenService;

        public TokenServiceTests_RefreshAsync()
        {
            _faker = new Faker();
            _userRepository = Substitute.For<IUserRepository>();
            _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
            _deviceRepository = Substitute.For<IDeviceRepository>();
            _dateTimeProvider = Substitute.For<IDateTimeProvider>();
            var expiry = TimeSpan.FromSeconds(1) + _faker.Date.Timespan(maxSpan: TimeSpan.FromSeconds(2)); // random 1÷3 seconds
            _authenticationOptions = Options.Create(new AuthenticationOptions()
            {
                TokenExpiry = expiry,
                RefreshTokenExpiry = 5 * expiry,
                SigningKey = _faker.Random.String2(32),
                Algorithm = _faker.PickRandom(SecurityAlgorithms.HmacSha256, SecurityAlgorithms.HmacSha512)
            });

            _tokenService = new TokenService(_userRepository, _refreshTokenRepository, _deviceRepository, _dateTimeProvider, _authenticationOptions);
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenDoesNotExistInRepository_ThrowsRefreshTokenInfoIdNotFoundException()
        {
            _dateTimeProvider.Now
                .Returns(_ => DateTime.UtcNow);
            _refreshTokenRepository.GetByIdAsync(Arg.Any<RefreshTokenInfoId>(), Arg.Any<CancellationToken>())
                .Returns((RefreshTokenInfo?)null);

            var user = _userFaker.Generate();
            var device = _deviceFaker.Generate();
            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            await Task.Delay(_authenticationOptions.Value.TokenExpiry);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<RefreshTokenInfoIdNotFoundException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenWasIssuedForDifferentToken_ThrowsTokenMismatchException()
        {
            var user = _userFaker.Generate();
            var device = _deviceFaker.Generate();
            _dateTimeProvider.Now
                .Returns(_ => DateTime.UtcNow);

            RefreshTokenInfo? refreshTokenInfo = null;
            _refreshTokenRepository
                .When(rtr => rtr.AddAsync(Arg.Any<RefreshTokenInfo>(), Arg.Any<CancellationToken>()))
                .Do(c =>
                {
                    var rti = c.Arg<RefreshTokenInfo>();
                    refreshTokenInfo = new RefreshTokenInfo(rti.Id, "jti_of_different_token", rti.UserId, rti.DeviceId, rti.Lifespan);
                });

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            await Task.Delay(_authenticationOptions.Value.TokenExpiry);

            _refreshTokenRepository.GetByIdAsync(Arg.Any<RefreshTokenInfoId>(), Arg.Any<CancellationToken>())
                .Returns(refreshTokenInfo);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<TokenMismatchException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenWasAlreadyInvalidated_ThrowsRefreshTokenInvalidatedException()
        {
            var user = _userFaker.Generate();
            var device = _deviceFaker.Generate();
            _dateTimeProvider.Now
                .Returns(_ => DateTime.UtcNow);

            RefreshTokenInfo? refreshTokenInfo = null;
            _refreshTokenRepository
                .When(rtr => rtr.AddAsync(Arg.Any<RefreshTokenInfo>(), Arg.Any<CancellationToken>()))
                .Do(c =>
                {
                    var rti = c.Arg<RefreshTokenInfo>();
                    rti.Invalidated = true;
                    refreshTokenInfo = rti;
                });

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            await Task.Delay(_authenticationOptions.Value.TokenExpiry);

            _refreshTokenRepository.GetByIdAsync(Arg.Any<RefreshTokenInfoId>(), Arg.Any<CancellationToken>())
                .Returns(refreshTokenInfo);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<RefreshTokenInvalidatedException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenWasAlreadyUsed_ThrowsRefreshTokenUsedException()
        {
            var user = _userFaker.Generate();
            var device = _deviceFaker.Generate();
            _dateTimeProvider.Now
                .Returns(_ => DateTime.UtcNow);

            RefreshTokenInfo? refreshTokenInfo = null;
            _refreshTokenRepository
                .When(rtr => rtr.AddAsync(Arg.Any<RefreshTokenInfo>(), Arg.Any<CancellationToken>()))
                .Do(c =>
                {
                    var rti = c.Arg<RefreshTokenInfo>();
                    rti.Used = true;
                    refreshTokenInfo = rti;
                });

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            await Task.Delay(_authenticationOptions.Value.TokenExpiry);

            _refreshTokenRepository.GetByIdAsync(Arg.Any<RefreshTokenInfoId>(), Arg.Any<CancellationToken>())
                .Returns(refreshTokenInfo);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<RefreshTokenUsedException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenTokenHasNotExpiredYet_ThrowsTokenNotExpiredException()
        {
            var user = _userFaker.Generate();
            var device = _deviceFaker.Generate();
            _dateTimeProvider.Now
                .Returns(_ => DateTime.UtcNow);

            RefreshTokenInfo? refreshTokenInfo = null;
            _refreshTokenRepository
                .When(rtr => rtr.AddAsync(Arg.Any<RefreshTokenInfo>(), Arg.Any<CancellationToken>()))
                .Do(c => refreshTokenInfo = c.Arg<RefreshTokenInfo>());

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            await Task.Delay(_authenticationOptions.Value.TokenExpiry.Milliseconds);

            _refreshTokenRepository.GetByIdAsync(Arg.Any<RefreshTokenInfoId>(), Arg.Any<CancellationToken>())
                .Returns(refreshTokenInfo);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<TokenNotExpiredException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenHasExpired_ThrowsRefreshTokenExpiredException()
        {
            var user = _userFaker.Generate();
            var device = _deviceFaker.Generate();
            _dateTimeProvider.Now
                .Returns(_ => DateTime.UtcNow);

            RefreshTokenInfo? refreshTokenInfo = null;
            _refreshTokenRepository
                .When(rtr => rtr.AddAsync(Arg.Any<RefreshTokenInfo>(), Arg.Any<CancellationToken>()))
                .Do(c => refreshTokenInfo = c.Arg<RefreshTokenInfo>());

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            await Task.Delay(_authenticationOptions.Value.RefreshTokenExpiry);

            _dateTimeProvider.Now
                .Returns(DateTime.UtcNow);
            _refreshTokenRepository.GetByIdAsync(Arg.Any<RefreshTokenInfoId>(), Arg.Any<CancellationToken>())
                .Returns(refreshTokenInfo);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<RefreshTokenExpiredException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenTokenPointsToNonExistingUser_ThrowsUserIdNotFoundException()
        {
            var user = _userFaker.Generate();
            var device = _deviceFaker.Generate();
            _dateTimeProvider.Now
                .Returns(_ => DateTime.UtcNow);

            RefreshTokenInfo? refreshTokenInfo = null;
            _refreshTokenRepository
                .When(rtr => rtr.AddAsync(Arg.Any<RefreshTokenInfo>(), Arg.Any<CancellationToken>()))
                .Do(c => refreshTokenInfo = c.Arg<RefreshTokenInfo>());

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            await Task.Delay(_authenticationOptions.Value.TokenExpiry);

            _refreshTokenRepository.GetByIdAsync(Arg.Any<RefreshTokenInfoId>(), Arg.Any<CancellationToken>())
                .Returns(refreshTokenInfo);
            _userRepository.GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
                .Returns((User?)null);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<UserIdNotFoundException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenPointsToDifferentUserThanToken_ThrowsUserIdMismatchException()
        {
            var user = _userFaker.Generate();
            var device = _deviceFaker.Generate();
            _dateTimeProvider.Now
                .Returns(_ => DateTime.UtcNow);

            RefreshTokenInfo? refreshTokenInfo = null;
            _refreshTokenRepository
                .When(rtr => rtr.AddAsync(Arg.Any<RefreshTokenInfo>(), Arg.Any<CancellationToken>()))
                .Do(c =>
                {
                    var rti = c.Arg<RefreshTokenInfo>();
                    var differentUser = _userFaker.Generate();
                    refreshTokenInfo = new RefreshTokenInfo(rti.Id, rti.Jti, differentUser.Id, rti.DeviceId, rti.Lifespan);
                });

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            await Task.Delay(_authenticationOptions.Value.TokenExpiry);

            _refreshTokenRepository.GetByIdAsync(Arg.Any<RefreshTokenInfoId>(), Arg.Any<CancellationToken>())
                .Returns(refreshTokenInfo);
            _userRepository.GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
                .Returns(user);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<UserIdMismatchException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenTokenPointsToNonExistingDevice_ThrowsDeviceIdNotFoundException()
        {
            var user = _userFaker.Generate();
            var device = _deviceFaker.Generate();
            _dateTimeProvider.Now
                .Returns(_ => DateTime.UtcNow);

            RefreshTokenInfo? refreshTokenInfo = null;
            _refreshTokenRepository
                .When(rtr => rtr.AddAsync(Arg.Any<RefreshTokenInfo>(), Arg.Any<CancellationToken>()))
                .Do(c => refreshTokenInfo = c.Arg<RefreshTokenInfo>());

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            await Task.Delay(_authenticationOptions.Value.TokenExpiry);

            _refreshTokenRepository.GetByIdAsync(Arg.Any<RefreshTokenInfoId>(), Arg.Any<CancellationToken>())
                .Returns(refreshTokenInfo);
            _userRepository.GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
                .Returns(user);
            _deviceRepository.GetByIdAsync(Arg.Any<DeviceId>(), Arg.Any<CancellationToken>())
                .Returns((Device?)null); ;

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<DeviceIdNotFoundException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenRefreshTokenPointsToDifferentDeviceThanToken_ThrowsDeviceIdMismatchException()
        {
            var user = _userFaker.Generate();
            var device = _deviceFaker.Generate();
            _dateTimeProvider.Now
                .Returns(_ => DateTime.UtcNow);

            RefreshTokenInfo? refreshTokenInfo = null;
            _refreshTokenRepository
                .When(rtr => rtr.AddAsync(Arg.Any<RefreshTokenInfo>(), Arg.Any<CancellationToken>()))
                .Do(c =>
                {
                    var rti = c.Arg<RefreshTokenInfo>();
                    var differentDevice = _deviceFaker.Generate();
                    refreshTokenInfo = new RefreshTokenInfo(rti.Id, rti.Jti, rti.UserId, differentDevice.Id, rti.Lifespan);
                });

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            await Task.Delay(_authenticationOptions.Value.TokenExpiry);

            _refreshTokenRepository.GetByIdAsync(Arg.Any<RefreshTokenInfoId>(), Arg.Any<CancellationToken>())
                .Returns(refreshTokenInfo);
            _userRepository.GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
                .Returns(user);
            _deviceRepository.GetByIdAsync(Arg.Any<DeviceId>(), Arg.Any<CancellationToken>())
                .Returns(device);

            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, device, CancellationToken.None);

            await refresh.Should().ThrowAsync<DeviceIdMismatchException>();
        }

        [Fact]
        public async Task RefreshAsync_WhenTryingToRefreshFromDifferentDeviceThanTokenWasIssuedFor_ThrowsDeviceIdMismatchException()
        {
            var user = _userFaker.Generate();
            var device = _deviceFaker.Generate();
            _dateTimeProvider.Now
                .Returns(_ => DateTime.UtcNow);

            RefreshTokenInfo? refreshTokenInfo = null;
            _refreshTokenRepository
                .When(rtr => rtr.AddAsync(Arg.Any<RefreshTokenInfo>(), Arg.Any<CancellationToken>()))
                .Do(c => refreshTokenInfo = c.Arg<RefreshTokenInfo>());

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            await Task.Delay(_authenticationOptions.Value.TokenExpiry);

            _refreshTokenRepository.GetByIdAsync(Arg.Any<RefreshTokenInfoId>(), Arg.Any<CancellationToken>())
                .Returns(refreshTokenInfo);
            _userRepository.GetByIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
                .Returns(user);
            _deviceRepository.GetByIdAsync(Arg.Any<DeviceId>(), Arg.Any<CancellationToken>())
                .Returns(device);

            var differentDevice = _deviceFaker.Generate();
            var refresh = async () => await _tokenService.RefreshAsync(tokenPair, differentDevice, CancellationToken.None);

            await refresh.Should().ThrowAsync<DeviceIdMismatchException>();
        }
    }
}
