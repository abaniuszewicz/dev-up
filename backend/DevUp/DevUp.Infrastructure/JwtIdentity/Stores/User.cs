using System;

namespace DevUp.Infrastructure.Identity.Stores
{
    internal class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
