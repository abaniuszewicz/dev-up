using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class DeviceNameTests
    {
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
        public void Equality_WhenCompared_ChecksByValue()
        {
            var deviceName1 = new DeviceName("valid-device-name");
            var deviceName2 = new DeviceName("valid-device-name");

            Assert.AreEqual(deviceName1, deviceName2);
        }
    }
}
