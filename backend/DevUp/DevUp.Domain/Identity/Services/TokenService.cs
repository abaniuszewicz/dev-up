using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevUp.Common;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services
{
    internal class TokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TokenService(IUserRepository userRepository, IDateTimeProvider dateTimeProvider)
        {
            _userRepository = userRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task ValidateAsync(Token token, RefreshToken refreshToken, Device device, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            var user = await _userRepository.GetByIdAsync(token.UserId, cancellationToken);
            if (user is null)
                errors.Add("Token did not contain id of an existing user");

            if (refreshToken.Invalidated)
                errors.Add("Refresh token has been invalidated");
            if (refreshToken.Used)
                errors.Add("Refresh token has been already used");
            if (!refreshToken.IsActive(_dateTimeProvider))
                errors.Add("Refresh token is no longer active");
            if (!refreshToken.BelongsTo(token))
                errors.Add("Refresh token does not belong to this token");
            if (!refreshToken.BelongsTo(device))
                errors.Add("Refresh token does not belong to this device");
            if (user is not null && !refreshToken.BelongsTo(user))
                errors.Add("Refresh token does not belong to this user");

            if (errors.Any())
                throw new IdentityException(errors);
        }
    }
}
