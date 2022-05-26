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
        private const string SecurityAlghoritm = SecurityAlgorithms.HmacSha256;

        private User _user;
        private JwtSettings _settings;
        private IDateTimeProvider _dateTimeProvider;

        public TokenBuilder ForUser(User user)
        {
            _user = user;
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

        public Token Build()
        {
            if (_user is null)
                throw new ArgumentNullException(nameof(_user));
            if (_settings is null)
                throw new ArgumentNullException(nameof(_settings));
            if (_dateTimeProvider is null)
                throw new ArgumentNullException(nameof(_dateTimeProvider));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("username", _user.Username.Value),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = _dateTimeProvider.UtcNow.AddMilliseconds(_settings.JwtExpiryMs),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_settings.Secret), SecurityAlghoritm)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return new Token(token);
        }
    }
}
