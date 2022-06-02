using System;
using System.Security.Cryptography;
using DevUp.Common;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Creation
{
    internal class RefreshTokenBuilder
    {
        private TokenInfo _token;
        private User _user;
        private Device _device;
        private IDateTimeProvider _dateTimeProvider;
        private JwtSettings _settings;

        public RefreshTokenBuilder FromTokenInfo(TokenInfo token)
        {
            _token = token;
            return this;
        }

        public RefreshTokenBuilder ForUser(User user)
        {
            _user = user;
            return this;
        }

        public RefreshTokenBuilder ForDevice(Device device)
        {
            _device = device;
            return this;
        }

        public RefreshTokenBuilder WithSettings(JwtSettings settings)
        {
            _settings = settings;
            return this;
        }

        public RefreshTokenBuilder WithTimeProvider(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            return this;
        }

        private static string GetRandomString()
        {
            var randomNumber = new byte[64];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public RefreshTokenInfo Build()
        {
            if (_token is null)
                throw new ArgumentNullException(nameof(_token));
            if (_user is null)
                throw new ArgumentNullException(nameof(_user));
            if (_device is null)
                throw new ArgumentNullException(nameof(_device));
            if (_settings is null)
                throw new ArgumentNullException(nameof(_settings));
            if (_dateTimeProvider is null)
                throw new ArgumentNullException(nameof(_dateTimeProvider));

            var randomString = GetRandomString();
            var id = new RefreshToken(randomString);
            var from = _dateTimeProvider.UtcNow;
            var to = _dateTimeProvider.UtcNow.AddMilliseconds(_settings.JwtRefreshExpiryMs);
            var lifespan = new DateTimeRange(from, to);
            return new RefreshTokenInfo(id, _token.Jti, _user.Id, _device.Id, lifespan);
        }
    }
}
