using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class RefreshTokenInfoIdNotFoundException : IdentityNotFoundException
    {
        public RefreshTokenInfoId RefreshTokenInfoId { get; }

        public RefreshTokenInfoIdNotFoundException(RefreshTokenInfoId refreshTokenInfoId) 
            : base("Refresh token info with this value does not exist.")
        {
            RefreshTokenInfoId = refreshTokenInfoId;
        }
    }
}
