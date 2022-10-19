namespace DevUp.Domain.Identity.Exceptions
{
    public interface IIdentityException
    {
        public bool CanLeak { get; }
    }
}
