using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects.Exceptions
{
    internal sealed class EmptyDeviceNameException : IdentityDataValidationException
    {
        public EmptyDeviceNameException()
            : base("Device name cannot be empty.")
        {
        }
    }
}
