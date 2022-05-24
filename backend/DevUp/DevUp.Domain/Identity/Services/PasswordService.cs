using System;
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
            throw new NotImplementedException();
        }

        public Task<PasswordVerifyResult> VerifyAsync(Password password, PasswordHash passwordHash)
        {
            throw new NotImplementedException();
        }
    }
}
