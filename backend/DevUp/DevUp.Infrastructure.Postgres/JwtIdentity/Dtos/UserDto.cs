using Microsoft.AspNet.Identity;

namespace DevUp.Infrastructure.Postgres.JwtIdentity.Dtos
{
    internal class UserDto : IUser<Guid>
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string PasswordHash { get; set; }
    }
}
