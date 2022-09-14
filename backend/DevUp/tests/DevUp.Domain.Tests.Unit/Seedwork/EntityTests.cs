using Bogus;
using DevUp.Domain.Seedwork;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Seedwork
{
    public class EntityTests
    {
        private class DummyId : EntityId
        {
            public int Value { get; init; }
            public override bool Equals(EntityId other) => other is DummyId id && id.Value == Value;
            public override int GetHashCode() => Value.GetHashCode();
        }

        private class Dummy : Entity<DummyId>
        {
            public string? String { get; set; }
            public Dummy(DummyId id) : base(id)
            {
            }
        }

        private readonly Faker<Dummy> _faker = new Faker<Dummy>()
            .RuleFor(d => d.String, f => f.Random.String2(5))
            .CustomInstantiator(f => new Dummy(new DummyId() { Value = f.Random.Number() }));


        [Test]
        public void Equality_ForTheSameIdValues_ReturnsTrue()
        {
            var dummy = _faker.Generate();
            var sameDummy = _faker.Clone()
                .CustomInstantiator(f => new Dummy(new DummyId() { Value = dummy.Id.Value }))
                .Generate();

            Assert.IsTrue(object.Equals(dummy, sameDummy));
            Assert.IsTrue(dummy.Equals(sameDummy));
            Assert.IsTrue(dummy == sameDummy);
            Assert.IsFalse(dummy != sameDummy);
        }

        [Test]
        public void Equality_ForDifferentIdValues_ReturnsFalse()
        {
            var dummy = _faker.Generate();
            var differentDummy = _faker.Clone()
                .CustomInstantiator(f => new Dummy(new DummyId() { Value = RandomDifferentNumber(f, dummy.Id.Value) }))
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
    }
}
