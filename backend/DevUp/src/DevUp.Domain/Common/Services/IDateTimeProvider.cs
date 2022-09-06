using System;

namespace DevUp.Domain.Common.Services
{
    public interface IDateTimeProvider
    {
        public DateTime Now { get; }
    }
}
