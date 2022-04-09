using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit
{
    public class PurpusefullyFailingTests
    {
        [Test]
        public void Fail()
        {
            Assert.Fail();
        }
    }
}
