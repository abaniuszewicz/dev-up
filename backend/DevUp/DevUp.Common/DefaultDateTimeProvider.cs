namespace DevUp.Common
{
    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow { get; } = DateTime.UtcNow;
    }
}
