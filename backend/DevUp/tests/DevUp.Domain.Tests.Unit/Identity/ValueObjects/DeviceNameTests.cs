using System.Collections.Generic;
using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class DeviceNameTests
    {
        private class Dummy : DeviceName
        {
            public Dummy(string deviceName) : base(deviceName)
            {
            }

            public new IEnumerable<object> GetEqualityComponents() => base.GetEqualityComponents();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Constructor_WhenGivenEmptyDeviceName_ThrowsEmptyDeviceNameException(string deviceName)
        {
            var exception = Assert.Throws<EmptyDeviceNameException>(() => new DeviceName(deviceName));
        }

        [Test]
        public void Constructor_WhenGivenValidDeviceName_AssignsValueProperty()
        {
            const string value = "valid-device-name";
            var deviceName = new DeviceName(value);
            Assert.AreEqual(deviceName.Value, value);
        }

        [Test]
        public void GetEqualityComponents_WhenCalled_ReturnsDeviceNameValue()
        {
            const string value = "valid-username";
            var deviceName = new Dummy(value);

            var result = deviceName.GetEqualityComponents();

            Assert.That(result, Has.One.EqualTo(value));
        }
    }
}
