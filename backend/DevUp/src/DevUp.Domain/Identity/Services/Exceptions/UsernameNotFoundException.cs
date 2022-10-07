using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public class UsernameNotFoundException : IdentityNotFoundException
    {
        public Username Username { get; }

        public UsernameNotFoundException(Username username) 
            : base("User with this username does not exist.")
        {
            Username = username;
        }
    }
}
