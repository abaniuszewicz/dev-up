using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Enums;
using DevUp.Domain.Identity.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace DevUp.Domain.Identity.Services
{
    internal class PasswordService : IPasswordService
    {
        private readonly IPasswordHasher<User> _passwordHasher;

        public PasswordService(IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public Task<PasswordHash> HashAsync(Password password, CancellationToken cancellationToken)
        {
            var hash = _passwordHasher.HashPassword(user: null, password.Value);
            return Task.FromResult(new PasswordHash(hash));
        }

        public Task<PasswordVerifyResult> VerifyAsync(Password password, PasswordHash passwordHash, CancellationToken cancellationToken)
        {
            var result = _passwordHasher.VerifyHashedPassword(user: null, passwordHash.Value, password.Value) switch
            {
                PasswordVerificationResult.Failed => PasswordVerifyResult.Failed,
                _ => PasswordVerifyResult.Success // todo: cover rehash
            };

            return Task.FromResult(result);
        }
    }
}
