using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class RefreshTokenUsedException : IdentityException
    {
        public RefreshTokenUsedException()
            : base("Refresh token has been already used.")
        {
        }
    }
}
