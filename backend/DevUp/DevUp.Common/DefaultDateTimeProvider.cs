namespace DevUp.Common
{
    internal class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow { get; } = DateTime.UtcNow;
    }
}
