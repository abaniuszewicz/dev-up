using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public class UsernameTakenException : IdentityBusinessRuleValidationException
    {
        public Username Username { get; }

        public UsernameTakenException(Username username) 
            : base("User with this username already exist.")
        {
            Username = username;
        }
    }
}
