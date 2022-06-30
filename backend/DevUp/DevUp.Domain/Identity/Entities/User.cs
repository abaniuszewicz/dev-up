using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Entities
{
    public class User : Entity<UserId>
    {
        public Username Username { get; }

        public User(UserId id, Username username) : base(id)
        {
            Username = username;
        }

        public override string ToString()
        {
            return Username.ToString();
        }
    }
}
