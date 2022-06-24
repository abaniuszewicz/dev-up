using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.Exceptions
{
    public class TokenValidationException : ValidationException
    {
        internal const string TokenNullMessage = "Token cannot be null.";
        internal const string TokenEmptyMessage = "Token cannot be empty.";
        internal const string TokenInvalidUserIdMessage = "Token did not contain id of an existing user";
        internal const string TokenNotActiveMessage = "Token is no longer active";

        internal const string RefreshTokenNullMessage = "Refresh token cannot be null.";
        internal const string RefreshTokenEmptyMessage = "Refresh token cannot be empty.";
        internal const string RefreshTokenNotActiveMessage = "Token is no longer active";
        internal const string RefreshTokenInvalidatedMessage = "Refresh token has been invalidated";
        internal const string RefreshTokenUsedMessage = "Refresh token has been already used";
        internal const string RefreshTokenWrongUserMessage = "Refresh token does not belong to this user";
        internal const string RefreshTokenWrongTokenMessage = "Refresh token does not belong to this token";
        internal const string RefreshTokenWrongDeviceMessage = "Refresh token does not belong to this device";

        public TokenValidationException(string error) : base(error)
        {
        }

        public TokenValidationException(IEnumerable<string> errors) : base(errors)
        {
        }
    }
}
