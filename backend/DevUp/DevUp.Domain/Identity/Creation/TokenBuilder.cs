using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DevUp.Common;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;
using Microsoft.IdentityModel.Tokens;

namespace DevUp.Domain.Identity.Creation
{
    internal class TokenBuilder
    {
        internal const string SecurityAlghoritm = SecurityAlgorithms.HmacSha256;

        private User _user;
        private Device _device; // TODO
        private JwtSettings _settings;
        private IDateTimeProvider _dateTimeProvider;

        public TokenBuilder ForUser(User user)
        {
            _user = user;
            return this;
        }

        public TokenBuilder ForDevice(Device device)
        {
            _device = device;
            return this;
        }

        public TokenBuilder WithSettings(JwtSettings settings)
        {
            _settings = settings;
            return this;
        }

        public TokenBuilder WithTimeProvider(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            return this;
        }

        public TokenInfo Build()
        {
            if (_user is null)
                throw new ArgumentNullException(nameof(_user));
            if (_settings is null)
                throw new ArgumentNullException(nameof(_settings));
            if (_dateTimeProvider is null)
                throw new ArgumentNullException(nameof(_dateTimeProvider));

            var tokenHandler = new JwtSecurityTokenHandler();
            var jti = Guid.NewGuid().ToString();
            var now = _dateTimeProvider.UtcNow;
            var lifespan = new DateTimeRange(now, now.AddMilliseconds(_settings.JwtExpiryMs));
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("sub", _user.Id.ToString()),
                    new Claim("jti", jti)
                }),
                NotBefore = lifespan.Start,
                Expires = lifespan.End,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_settings.Secret), SecurityAlghoritm)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(securityToken);
            var token = new Token(jwt);
            return new TokenInfo(token, jti, _user.Id, lifespan);
        }
    }
}
