namespace DevUp.Infrastructure.Postgres.JwtIdentity.Results
{
    public abstract class JwtResult
    {
        public string Token { get; init; }
        public string RefreshToken { get; init; }
    }
}
