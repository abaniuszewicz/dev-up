using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Enums;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services
{
    internal class PasswordService : IPasswordService
    {
        public Task<PasswordHash> HashAsync(Password password, CancellationToken cancellationToken)
        {
            return Task.FromResult(new PasswordHash(password.Value));
        }

        public Task<PasswordVerifyResult> VerifyAsync(Password password, PasswordHash passwordHash, CancellationToken cancellationToken)
        {
            return Task.FromResult(PasswordVerifyResult.Success);
        }
    }
}
