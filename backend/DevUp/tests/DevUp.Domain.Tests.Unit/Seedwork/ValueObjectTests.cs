using System.Collections.Generic;
using Bogus;
using DevUp.Domain.Seedwork;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Seedwork
{
    public class ValueObjectTests
    {
        public class Dummy : ValueObject
        {
            public int Int { get; init; }
            public string? String { get; init; }

            protected override IEnumerable<object?> GetEqualityComponents()
            {
                yield return Int;
                /* note that even there is a String property
                 * it's not being used in equality comparison */
            }
        }

        private readonly Faker<Dummy> _faker = new Faker<Dummy>()
            .RuleFor(d => d.String, f => f.Random.String2(5))
            .RuleFor(d => d.Int, f => f.Random.Number());


        [Test]
        public void Equality_ForObjectsThatHaveTheSameValuesInComponentsUsedForComparison_ReturnsTrue()
        {
            var dummy = _faker.Generate();
            var sameDummy = _faker.Clone()
                .RuleFor(d => d.Int, _ => dummy.Int)
                .RuleFor(d => d.String, f => RandomDifferentString(f, dummy.String!)) // will be different but it's not being used in comparison
                .Generate();

            Assert.IsTrue(object.Equals(dummy, sameDummy));
            Assert.IsTrue(dummy.Equals(sameDummy));
            Assert.IsTrue(dummy == sameDummy);
            Assert.IsFalse(dummy != sameDummy);
        }

        [Test]
        public void Equality_ForObjectsThatHaveDifferentValuesInComponentsUsedForComparison_ReturnsFalse()
        {
            var dummy = _faker.Generate();
            var differentDummy = _faker.Clone()
                .RuleFor(d => d.Int, f => RandomDifferentNumber(f, dummy.Int))
                .RuleFor(d => d.String, _ => dummy.String) // will be the same but it's not being used in comparison
                .Generate();

            Assert.IsFalse(object.Equals(dummy, differentDummy));
            Assert.IsFalse(dummy.Equals(differentDummy));
            Assert.IsFalse(dummy == differentDummy);
            Assert.IsTrue(dummy != differentDummy);
        }

        private static int RandomDifferentNumber(Faker faker, int number)
        {
            var differentNumber = faker.Random.Number();
            while (number == differentNumber)
                differentNumber = faker.Random.Number();

            return differentNumber;
        }

        private static string RandomDifferentString(Faker faker, string @string)
        {
            var differentString = faker.Random.String2(@string.Length);
            while (@string == differentString)
                differentString = faker.Random.String2(@string.Length);

            return differentString;
        }
    }
}
