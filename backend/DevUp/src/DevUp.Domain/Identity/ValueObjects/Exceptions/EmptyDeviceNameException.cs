using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects.Exceptions
{
    public sealed class EmptyDeviceNameException : IdentityDataValidationException
    {
        public EmptyDeviceNameException()
            : base("Device name cannot be empty.")
        {
        }
    }
}
