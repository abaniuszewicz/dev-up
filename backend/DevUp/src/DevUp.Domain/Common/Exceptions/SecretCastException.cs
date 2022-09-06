using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Common.Exceptions
{
    public class SecretCastException<T> : DomainException
    {
        public SecretCastException(string key)
            : base($"Failed to cast secret {key} to type {typeof(T).Name}")
        {
        }
    }
}
