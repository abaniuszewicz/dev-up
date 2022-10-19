using System.Collections.Generic;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public class DeviceName : ValueObject
    {
        public string Value { get; }

        public DeviceName(string name)
        {
            Validate(name);
            Value = name;
        }

        private static void Validate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new EmptyDeviceNameException();
        }

        public override string ToString()
        {
            return Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
