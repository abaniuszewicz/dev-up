using System.Collections.Generic;
using DevUp.Domain.Organization.ValueObjects.Exceptions;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Organization.ValueObjects
{
    public sealed class TeamName : ValueObject
    {
        public string Value { get; }

        public TeamName(string name)
        {
            Validate(name);
            Value = name;
        }

        private void Validate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new EmptyTeamNameException();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
