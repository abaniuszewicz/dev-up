namespace DevUp.Application.Identity
{
    public class TokenPair
    {
        public string Token { get; }
        public string RefreshToken { get; }

        public TokenPair(string token, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }
    }
}
