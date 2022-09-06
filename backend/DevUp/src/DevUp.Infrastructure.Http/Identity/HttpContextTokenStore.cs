using DevUp.Application.Identity;
using DevUp.Infrastructure.Http.Identity.Exceptions;
using Microsoft.AspNetCore.Http;

namespace DevUp.Infrastructure.Http.Identity
{
    internal class HttpContextTokenStore : ITokenStore
    {
        private const string Key = "token_store";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextTokenStore(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public TokenPair Get()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context is null)
                throw new GetTokenException("Http context is null.");
            if (!context.Items.TryGetValue(Key, out var value))
                throw new GetTokenException("Token pair was not stored yet.");
            if (value is not TokenPair tokenPair)
                throw new GetTokenException("Invalid value was stored under that key.");

            return tokenPair;
        }

        public void Set(TokenPair tokenPair)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context is null)
                throw new SetTokenException("Http context is null.");
            if (!context.Items.TryAdd(Key, tokenPair))
                throw new SetTokenException("Token pair was already stored.");
        }
    }
}
