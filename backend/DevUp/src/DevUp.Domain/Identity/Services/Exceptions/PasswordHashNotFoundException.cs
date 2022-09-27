using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public class PasswordHashNotFoundException : IdentityNotFoundException
    {
        public Username Username { get; }

        public PasswordHashNotFoundException(Username username) 
            : base("User with this username has no password hash associated.")
        {
            Username = username;
        }
    }
}
