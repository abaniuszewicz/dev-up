using DevUp.Infrastructure.Exceptions;
using DevUp.Infrastructure.Postgres.Identity.Dtos;

namespace DevUp.Infrastructure.Postgres.Identity.Repositories.Exceptions
{
    internal class UserNotPersistedException : InfrastructureException
    {
        public UserDto User { get; }

        public UserNotPersistedException(UserDto user) 
            : base("Failed to persist the user.")
        {
            User = user;
        }
    }
}
