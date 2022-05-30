namespace DevUp.Infrastructure.Postgres.JwtIdentity.Dtos
{
    internal class RefreshTokenDto
    {
        public string Token { get; set; }
        public string Jti { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DeviceDto Device { get; set; }
        public bool Used { get; set; }
        public bool Invalidated { get; set; }
    }
}
