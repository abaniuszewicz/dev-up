using System;
using DevUp.Common;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Creation
{
    internal class RefreshTokenBuilder
    {
        private Token _token;
        private User _user;
        private IDateTimeProvider _dateTimeProvider;
        private JwtSettings _settings;

        public RefreshTokenBuilder FromToken(Token token)
        {
            _token = token;
            return this;
        }

        public RefreshTokenBuilder ForUser(User user)
        {
            _user = user;
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

        public RefreshToken Build()
        {
            if (_token is null)
                throw new ArgumentNullException(nameof(_token));
            if (_user is null)
                throw new ArgumentNullException(nameof(_user));
            if (_settings is null)
                throw new ArgumentNullException(nameof(_settings));
            if (_dateTimeProvider is null)
                throw new ArgumentNullException(nameof(_dateTimeProvider));

            var id = new RefreshTokenId();
            var from = _dateTimeProvider.UtcNow;
            var to = _dateTimeProvider.UtcNow.AddMilliseconds(_settings.JwtRefreshExpiryMs);
            var lifespan = new DateTimeRange(from, to);
            return new RefreshToken(id, _token, _user.Id, lifespan);
        }
    }
}
