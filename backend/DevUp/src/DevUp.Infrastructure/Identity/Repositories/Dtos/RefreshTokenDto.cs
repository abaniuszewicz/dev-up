using DevUp.Domain.Identity.Entities;

namespace DevUp.Infrastructure.Identity.Repositories.Dtos
{
    internal record RefreshTokenDto
    {
        public RefreshTokenInfo RefreshTokenInfo { get; init; }
        public RefreshTokenInfoId Previous { get; set; }
        public RefreshTokenInfoId Next { get; set; }
    }
}
