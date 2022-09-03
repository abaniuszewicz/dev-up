using System;
using DevUp.Domain.Common.Types;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Common.Types
{
    public class DateTimeRangeTests
    {
        private static readonly DateTime Jan1999 = new DateTime(1999, 1, 1);
        private static readonly DateTime Jan2000 = new DateTime(2000, 1, 1);

        [Test]
        public void Constructor_WhenCalledWithEndDateBeforeStartDate_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new DateTimeRange(start: Jan2000, end: Jan1999));
        }

        [Test]
        public void Constructor_WhenCalledWithCorrectStartAndEndDate_AssignsStartAndEndProperties()
        {
            var range = new DateTimeRange(start: Jan1999, end: Jan2000);
            Assert.AreEqual(Jan1999, range.Start);
            Assert.AreEqual(Jan2000, range.End);
        }

        [Test]
        public void IsWithinRange_ForDatesWithinRange_ReturnsTrue()
        {
            var range = new DateTimeRange(start: Jan1999, end: Jan2000);
            var jun1999 = new DateTime(1999, 6, 1);

            Assert.IsTrue(range.IsWithinRange(Jan1999));
            Assert.IsTrue(range.IsWithinRange(jun1999));
            Assert.IsTrue(range.IsWithinRange(Jan2000));
        }

        [Test]
        public void IsWithinRange_ForDatesOutsideRange_ReturnsFalse()
        {
            var range = new DateTimeRange(start: Jan1999, end: Jan2000);
            var dec1998 = new DateTime(1998, 12, 31);
            var feb2000 = new DateTime(2000, 2, 1);

            Assert.IsFalse(range.IsWithinRange(dec1998));
            Assert.IsFalse(range.IsWithinRange(feb2000));
        }

        [Test]
        public void IsWithinRange_WhenCalled_TakesTimeComponentIntoConsideration()
        {
            var before = new DateTime(2000, 1, 1, hour: 1, minute: 1, second: 1, millisecond: 0);
            var start = new DateTime(2000, 1, 1, hour: 1, minute: 1, second: 1, millisecond: 1);
            var between = new DateTime(2000, 1, 1, hour: 1, minute: 1, second: 1, millisecond: 2);
            var end = new DateTime(2000, 1, 1, hour: 1, minute: 1, second: 1, millisecond: 3);
            var after = new DateTime(2000, 1, 1, hour: 1, minute: 1, second: 1, millisecond: 4);

            var range = new DateTimeRange(start, end);
            Assert.IsFalse(range.IsWithinRange(before));
            Assert.IsTrue(range.IsWithinRange(between));
            Assert.IsFalse(range.IsWithinRange(after));
        }
    }
}
