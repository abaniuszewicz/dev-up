using System;

namespace DevUp.Infrastructure.Exceptions
{
    public class InfrastructureException : Exception
    {
        public InfrastructureException(string message)
            : base(message)
        {
        }
    }
}
