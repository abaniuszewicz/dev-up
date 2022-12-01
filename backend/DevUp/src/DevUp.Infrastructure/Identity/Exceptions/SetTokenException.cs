using DevUp.Infrastructure.Exceptions;

namespace DevUp.Infrastructure.Identity.Exceptions
{
    internal class SetTokenException : InfrastructureException
    {
        public string Reason { get; }

        public SetTokenException(string reason)
            : base($"Failed to store token pair: {reason}")
        {
            Reason = reason;
        }
    }
}
