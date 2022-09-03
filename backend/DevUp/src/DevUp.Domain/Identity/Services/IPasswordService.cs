using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Enums;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services
{
    public interface IPasswordService
    {
        public Task<PasswordHash> HashAsync(Password password, CancellationToken cancellationToken);
        public Task<PasswordVerifyResult> VerifyAsync(Password password, PasswordHash passwordHash, CancellationToken cancellationToken);
    }
}
