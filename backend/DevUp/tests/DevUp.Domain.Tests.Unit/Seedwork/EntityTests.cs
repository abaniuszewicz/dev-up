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

        [Test]
        public void Equality_WhenCompared_ChecksById()
        {
            var dummy =          new Dummy(new DummyId() { Value = 420 }) { String = "doesn't matter" };
            var sameDummy =      new Dummy(new DummyId() { Value = 420 }) { String = "matters not" };
            var differentDummy = new Dummy(new DummyId() { Value = 1337 }) { String = "doesn't matter" };

            Assert.IsTrue(object.Equals(dummy, sameDummy));
            Assert.IsTrue(dummy.Equals(sameDummy));
            Assert.IsTrue(dummy == sameDummy);
            Assert.IsFalse(dummy != sameDummy);

            Assert.IsFalse(object.Equals(dummy, differentDummy));
            Assert.IsFalse(dummy.Equals(differentDummy));
            Assert.IsFalse(dummy == differentDummy);
            Assert.IsTrue(dummy != differentDummy);
        }
    }
}
