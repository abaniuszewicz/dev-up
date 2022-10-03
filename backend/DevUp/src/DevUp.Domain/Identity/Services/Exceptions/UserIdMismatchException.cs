using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class UserIdMismatchException : IdentityException
    {
        public UserId UserIdFromToken { get; }
        public UserId UserIdFromRefreshToken { get; }

        public UserIdMismatchException(UserId userIdFromToken, UserId userIdFromRefreshToken)
            : base("Mismatch on user id from token and refresh token.")
        {
            UserIdFromToken = userIdFromToken;
            UserIdFromRefreshToken = userIdFromRefreshToken;
        }
    }
}
