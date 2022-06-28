using System;
using System.Collections.Generic;
using DevUp.Domain.Common.Extensions;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Common.Extensions
{
    public class IEnumerableExtensionsTests
    {
        private static readonly int[] Empty = Array.Empty<int>();
        private static readonly int[] OneToThree = new[] { 1, 2, 3 };

        [Test]
        public void ForEach_WhenCollectionIsEmpty_DoesntThrow()
        {
            Assert.DoesNotThrow(() => Empty.ForEach(i => { }));
        }

        [Test]
        public void ForEach_WhenCalled_SequentiallyExecutesActionOnEachItem()
        {
            var output = new List<int>();

            OneToThree.ForEach(output.Add);

            CollectionAssert.AreEqual(output, OneToThree);
        }

        [Test]
        public void None_WhenCollectionIsEmpty_ReturnsTrue()
        {
            Assert.IsTrue(Empty.None());
        }

        [Test]
        public void None_WhenCollectionIsNotEmpty_ReturnsFalse()
        {
            Assert.IsFalse(OneToThree.None());
        }

        [Test]
        public void None_WhenCollectionContainsNoMatchingItems_ReturnsTrue()
        {
            static bool IsFour(int i) => i == 4;
            Assert.IsTrue(OneToThree.None(IsFour));
        }

        [Test]
        public void None_WhenCollectionContainsMatchingItems_ReturnsFalse()
        {
            static bool IsThree(int i) => i == 3;
            Assert.IsFalse(OneToThree.None(IsThree));
        }
    }
}
