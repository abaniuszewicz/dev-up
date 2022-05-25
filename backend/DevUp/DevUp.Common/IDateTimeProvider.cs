namespace DevUp.Common
{
    public interface IDateTimeProvider
    {
        public DateTime UtcNow { get; }
    }
}
