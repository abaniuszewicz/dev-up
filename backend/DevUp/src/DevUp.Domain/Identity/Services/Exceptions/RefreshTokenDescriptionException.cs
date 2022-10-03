using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class RefreshTokenDescriptionException : IdentityValidationException
    {
        public RefreshTokenDescriptionException() 
            : base("Failed to describe refresh token.")
        {
        }
    }
}
