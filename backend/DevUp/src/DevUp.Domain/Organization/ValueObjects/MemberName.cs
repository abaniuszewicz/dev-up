using System.Collections.Generic;
using DevUp.Domain.Organization.ValueObjects.Exceptions;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Organization.ValueObjects
{
    public sealed class MemberName : ValueObject
    {
        public string Value { get; }

        public MemberName(string name)
        {
            Validate(name);
            Value = name;
        }

        private void Validate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new EmptyMemberNameException();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
