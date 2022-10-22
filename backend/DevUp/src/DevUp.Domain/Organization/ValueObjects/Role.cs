using System.Collections.Generic;
using DevUp.Domain.Organization.ValueObjects.Exceptions;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Organization.ValueObjects
{
    public sealed class Role : ValueObject
    {
        public string Value { get; }

        public Role(string role)
        {
            Validate(role);
            Value = role;
        }

        private void Validate(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new EmptyRoleException();
        }


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
