using System;

namespace DevUp.Infrastructure.Identity.Stores
{
    internal class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
