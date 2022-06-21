using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.Exceptions
{
    public class TokenValidationException : ValidationException
    {
        internal const string NullMessage = "Token cannot be null.";
        internal const string EmptyMessage = "Token cannot be empty.";

        public TokenValidationException(string error) : base(error)
        {
        }

        public TokenValidationException(IEnumerable<string> errors) : base(errors)
        {
        }
    }
}
