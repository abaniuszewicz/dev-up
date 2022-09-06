using System.Collections.Generic;

namespace DevUp.Domain.Identity.Exceptions
{
    internal sealed class RefreshException : IdentityValidationException
    {
        internal static string InvalidTokenMessage = "Invalid token.";
        internal static string InvalidRefreshTokenMessage = "Invalid refresh token.";

        public RefreshException(string error) 
            : base(error)
        {
        }

        public RefreshException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}
