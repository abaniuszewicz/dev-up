using System.Threading;
using System.Threading.Tasks;
using Bogus;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.Services.Exceptions;
using DevUp.Domain.Identity.ValueObjects;
using FluentAssertions;
using Xunit;

namespace DevUp.Domain.Tests.Integration.Identity.Services
{
    public class TokenServiceTests_RevokeAsync : IClassFixture<TokenServiceFactory>
    {
        private readonly Faker<Device> _deviceFaker = new Faker<Device>().CustomInstantiator(f => new(new(f.Random.Guid()), new($"{f.Person.FirstName} (iPhone)")));
        private readonly Faker<Username> _usernameFaker = new Faker<Username>().CustomInstantiator(f => new(f.Internet.UserName()));
        private readonly Faker<PasswordHash> _passwordHashFaker = new Faker<PasswordHash>().CustomInstantiator(f => new(f.Random.Hash()));

        private readonly TokenServiceFactory _tokenServiceFactory;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ITokenService _tokenService;

        public TokenServiceTests_RevokeAsync(TokenServiceFactory tokenServiceFactory)
        {
            _tokenServiceFactory = tokenServiceFactory;
            _userRepository = tokenServiceFactory.Create<IUserRepository>();
            _refreshTokenRepository = tokenServiceFactory.Create<IRefreshTokenRepository>();
            _deviceRepository = tokenServiceFactory.Create<IDeviceRepository>();
            _tokenService = tokenServiceFactory.Create<ITokenService>();
        }

        [Fact]
        public async Task RevokeAsync_WhenRefreshTokenWasNotFoundRepository_ThrowsRefreshTokenInfoIdNotFoundException()
        {
            var refreshToken = new RefreshToken("i-don't-exist");

            var refresh = async () => await _tokenService.RevokeAsync(refreshToken, CancellationToken.None);

            await refresh.Should().ThrowAsync<RefreshTokenInfoIdNotFoundException>();
        }

        [Fact]
        public async Task RevokeAsync_WhenCalledForChainWithOnlyOneToken_InvalidatesThatSingleToken()
        {
            var user = await _userRepository.CreateAsync(_usernameFaker, _passwordHashFaker, CancellationToken.None);
            var tokenPair = await CreateTokenPair(user, _deviceFaker);

            var rtiBeforeInvalidation = await _refreshTokenRepository.GetByIdAsync(new(tokenPair.RefreshToken), CancellationToken.None);
            rtiBeforeInvalidation.Invalidated.Should().BeFalse();

            await _tokenService.RevokeAsync(tokenPair.RefreshToken, CancellationToken.None);

            var rtiAfterInvalidation = await _refreshTokenRepository.GetByIdAsync(new(tokenPair.RefreshToken), CancellationToken.None);
            rtiAfterInvalidation.Invalidated.Should().BeTrue();
        }

        [Fact]
        public async Task RevokeAsync_WhenCalledForChainWithMultipleTokens_InvalidatesAllThatWereIssuedAfterRequestedTokenToInvalidate()
        {
            var user = await _userRepository.CreateAsync(_usernameFaker, _passwordHashFaker, CancellationToken.None);
            var device = _deviceFaker.Generate();

            var pair1 = await CreateTokenPair(user, device); // should not get invalidated
            var pair2 = await RefreshTokenPair(pair1, device); // should not get invalidated
            var pair3 = await RefreshTokenPair(pair2, device); // <--- invalidate this, should get invalidated
            var pair4 = await RefreshTokenPair(pair3, device); // should get invalidated
            var pair5 = await RefreshTokenPair(pair4, device); // should get invalidated

            await _tokenService.RevokeAsync(pair3.RefreshToken, CancellationToken.None);

            var rti1 = await _refreshTokenRepository.GetByIdAsync(new(pair1.RefreshToken), CancellationToken.None);
            rti1.Invalidated.Should().BeFalse();
            var rti2 = await _refreshTokenRepository.GetByIdAsync(new(pair2.RefreshToken), CancellationToken.None);
            rti2.Invalidated.Should().BeFalse();
            var rti3 = await _refreshTokenRepository.GetByIdAsync(new(pair3.RefreshToken), CancellationToken.None);
            rti3.Invalidated.Should().BeTrue();
            var rti4 = await _refreshTokenRepository.GetByIdAsync(new(pair4.RefreshToken), CancellationToken.None);
            rti4.Invalidated.Should().BeTrue();
            var rti5 = await _refreshTokenRepository.GetByIdAsync(new(pair5.RefreshToken), CancellationToken.None);
            rti5.Invalidated.Should().BeTrue();
        }

        [Fact]
        public async Task RevokeAsync_WhenThereAreMultipleChainsForTheSameUserAndDevice_InvalidatesOnlyThatChainThatWasRequestedAndLeavesOtherIntact()
        {
            var user = await _userRepository.CreateAsync(_usernameFaker, _passwordHashFaker, CancellationToken.None);
            var device = _deviceFaker.Generate();

            var chain1_pair1 = await CreateTokenPair(user, device);
            var chain1_pair2 = await RefreshTokenPair(chain1_pair1, device);

            var chain2_pair1 = await CreateTokenPair(user, device);
            var chain2_pair2 = await RefreshTokenPair(chain2_pair1, device);

            await _tokenService.RevokeAsync(chain1_pair1.RefreshToken, CancellationToken.None);

            var chain1_rti1 = await _refreshTokenRepository.GetByIdAsync(new(chain1_pair1.RefreshToken), CancellationToken.None);
            chain1_rti1.Invalidated.Should().BeTrue();
            var chain1_rti2 = await _refreshTokenRepository.GetByIdAsync(new(chain1_pair2.RefreshToken), CancellationToken.None);
            chain1_rti2.Invalidated.Should().BeTrue();

            var chain2_rti1 = await _refreshTokenRepository.GetByIdAsync(new(chain2_pair1.RefreshToken), CancellationToken.None);
            chain2_rti1.Invalidated.Should().BeFalse();
            var chain2_rti2 = await _refreshTokenRepository.GetByIdAsync(new(chain2_pair2.RefreshToken), CancellationToken.None);
            chain2_rti2.Invalidated.Should().BeFalse();
        }

        private async Task<TokenPair> CreateTokenPair(User user, Device device)
        {
            await _deviceRepository.AddAsync(device, CancellationToken.None);

            var tokenPair = await _tokenService.CreateAsync(user.Id, device.Id, CancellationToken.None);
            await Task.Delay(_tokenServiceFactory.AuthenticationOptions.TokenExpiry);
            return tokenPair;
        }

        private async Task<TokenPair> RefreshTokenPair(TokenPair oldTokenPair, Device device)
        {
            var newTokenPair = await _tokenService.RefreshAsync(oldTokenPair, device, CancellationToken.None);
            await Task.Delay(_tokenServiceFactory.AuthenticationOptions.TokenExpiry);
            return newTokenPair;
        }
    }
}
