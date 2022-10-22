namespace DevUp.Infrastructure.Postgres.Identity.Dtos
{
    internal record UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
