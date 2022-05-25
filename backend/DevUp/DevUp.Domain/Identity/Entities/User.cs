using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Entities
{
    public class User : Entity<UserId>
    {
        public Username Username { get; init; }

        public User(UserId id) : base(id)
        {
        }
    }
}
