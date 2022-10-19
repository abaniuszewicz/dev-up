namespace DevUp.Infrastructure.Postgres.Identity.Dtos
{
    internal record RefreshTokenDto
    {
        public string Previous { get; set; }
        public string Token { get; set; }
        public string Next { get; set; }
        public string Jti { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public Guid DeviceId { get; set; }
        public bool Used { get; set; }
        public bool Invalidated { get; set; }
    }
}
