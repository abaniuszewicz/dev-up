namespace DevUp.Application.Identity
{
    public interface ITokenStore
    {
        public void Set(TokenPair result);
        public TokenPair Get();
    }
}
