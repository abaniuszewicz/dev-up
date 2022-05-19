using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Entities
{
    public class User : Entity<UserId>
    {
        public string Username { get; }

        public User(UserId id) : base(id)
        {
        }
    }
}
