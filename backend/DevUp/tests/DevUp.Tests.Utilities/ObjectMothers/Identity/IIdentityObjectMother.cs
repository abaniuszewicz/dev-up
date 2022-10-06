using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Tests.Utilities.ObjectMothers.Identity
{
    public interface IIdentityObjectMother
    {
        public DeviceId DeviceId { get; }
        public Device Device { get; }
        public UserId UserId { get; }
        public Username Username { get; }
        public Password Password { get; }
        public PasswordHash PasswordHash { get; }
        public User User { get; }
        public Token Token { get; }
        public RefreshToken RefreshToken { get; }
    }
}
