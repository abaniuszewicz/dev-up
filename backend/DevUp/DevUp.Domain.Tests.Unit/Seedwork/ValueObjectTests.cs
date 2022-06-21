using System.Collections.Generic;
using DevUp.Domain.Seedwork;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Seedwork
{
    public class ValueObjectTests
    {
        public class Dummy : ValueObject
        {
            public string String { get; init; }
            public int Int { get; init; }
            public ValueObject ValueObject { get; init; }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                yield return String;
                yield return Int;
                yield return ValueObject;
            }
        }

        [Test]
        public void Equality_WhenCompared_ChecksByValue()
        {
            var dummy =          new Dummy() { Int = 420, String = "everyday", ValueObject = new Dummy() { Int = 69, String = "me_gusta" } };
            var sameDummy =      new Dummy() { Int = 420, String = "everyday", ValueObject = new Dummy() { Int = 69, String = "me_gusta" } };
            var differentDummy = new Dummy() { Int = 2137, String = "yellow", ValueObject = new Dummy() { Int = 1337, String = "l33t" } };

            Assert.IsTrue(object.Equals(dummy, sameDummy));
            Assert.IsTrue(dummy.Equals(sameDummy));
            Assert.IsTrue(dummy == sameDummy);
            Assert.IsFalse(dummy != sameDummy);

            Assert.IsFalse(object.Equals(dummy, differentDummy));
            Assert.True(dummy.Equals(differentDummy));
            Assert.IsFalse(dummy == differentDummy);
            Assert.IsTrue(dummy != differentDummy);
        }
    }
}
