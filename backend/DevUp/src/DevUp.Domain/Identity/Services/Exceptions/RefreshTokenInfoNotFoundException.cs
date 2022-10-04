using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class RefreshTokenInfoNotFoundException : IdentityNotFoundException
    {
        public RefreshToken RefreshToken { get; }

        public RefreshTokenInfoNotFoundException(RefreshToken refreshToken) 
            : base("Refresh token info with this value does not exist.")
        {
            RefreshToken = refreshToken;
        }
    }
}
