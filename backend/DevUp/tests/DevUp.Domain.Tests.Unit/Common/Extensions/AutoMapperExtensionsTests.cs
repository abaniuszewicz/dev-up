using AutoMapper;
using DevUp.Domain.Common.Extensions;
using Moq;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Common.Extensions
{
    public class AutoMapperExtensionsTests
    {
        private Mock<IMapper> _mock;
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _mock = new Mock<IMapper>();
            _mapper = _mock.Object;
        }

        [Test]
        public void MapOrNull_WhenGivenNull_DoesntForwardCallAndReturnsNull()
        {
            object @object = null;

            var result = _mapper.MapOrNull<object>(@object);

            _mock.VerifyNoOtherCalls();
            Assert.IsNull(result);
        }

        [Test]
        public void MapOrNull_WhenGivenNonNull_ForwardsCall()
        {
            object @object = new { };

            _mapper.MapOrNull<object>(@object);

            _mock.Verify(m => m.Map<object>(@object), Times.Once);
        }
    }
}
