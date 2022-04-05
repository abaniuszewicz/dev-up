namespace DevUp.Domain.Entities.Identity
{
    public class User : Entity<UserId>
    {
        public string Username { get; set; }

        public User(UserId id) : base(id)
        {
        }
    }
}
