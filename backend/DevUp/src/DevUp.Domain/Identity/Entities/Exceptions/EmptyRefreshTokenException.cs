using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Entities.Exceptions
{
    public sealed class EmptyRefreshTokenException : IdentityDataValidationException
    {
        public EmptyRefreshTokenException() 
            : base("Refresh token cannot be empty.")
        {
        }
    }
}
