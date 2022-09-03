using System;

namespace DevUp.Domain.Common.Services
{
    public class UtcDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now { get => DateTime.UtcNow; }
    }
}
