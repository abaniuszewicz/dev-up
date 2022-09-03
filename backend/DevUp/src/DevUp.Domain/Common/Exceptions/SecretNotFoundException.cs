using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Common.Exceptions
{
    public class SecretNotFoundException : DomainException
    {
        public SecretNotFoundException(string key)
            : base($"Secret {key} was not found")
        {
        }
    }
}
