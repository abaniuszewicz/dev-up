using DevUp.Infrastructure.Exceptions;

namespace DevUp.Infrastructure.Identity.Exceptions
{
    internal class GetTokenException : InfrastructureException
    {
        public string Reason { get; }

        public GetTokenException(string reason)
            : base($"Failed to retrieve token pair: {reason}")
        {
            Reason = reason;
        }
    }
}
