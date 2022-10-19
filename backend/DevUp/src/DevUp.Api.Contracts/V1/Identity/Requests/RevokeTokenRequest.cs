namespace DevUp.Api.Contracts.V1.Identity.Requests
{
    public class RevokeTokenRequest
    {
        /// <example>JCzXfzfZ/n97d9qQ9z1rvrAeOXikMns8jimyDzpqtg9gMWUrz3OcqqqthxpUG+9WaSFV1LdtWH4x7aDqBJ21gg==</example>
        public string RefreshToken { get; init; }
    }
}
