using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Tests.Utilities.ObjectMothers.Identity
{
    internal class SerenaWilliamsIdentityObjectMother : IIdentityObjectMother
    {
        public DeviceId DeviceId => new DeviceId("4bc5456a-074b-40cd-a582-8e30a0588a88");
        public Device Device => new Device(DeviceId, "DESKTOP-32fIo08");
        public UserId UserId => new UserId(Guid.Parse("956cd84f-0683-4a29-9ee4-1198829395dc"));
        public Username Username => new Username("serena-williams");
        public Password Password => new Password("$tr0nG-p4$$w0rD");
        public PasswordHash PasswordHash => new PasswordHash("AQAAAAEAACcQAAAAEGrReji9KHBVOY23KdhJxiOz3f//Fj0EOSIq9PTPhp9Vb3PxTpBVwxWchVy9j97fAQ==");
        public User User => new User(UserId, Username);
        public Token Token => new Token("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJzdWIiOiI5NWU3NjdlZi05N2JjLTQ2OTgtYjA2Yy02MTdiY2Y0ZWU4NDkiLCJqdGkiOiI0MDZlYmU2YS1lYTJlLTQ1ZDYtODYzOC1hYjYwYzBjYzU3ZTciLCJuYmYiOjE2NTU5MTAzNjksImV4cCI6MTY1NTkxMDY2OSwiaWF0IjoxNjU1OTEwMzc1fQ.OAZiZ37F9o4-dgxqRnXKVJVSnwQOp9a8RCWzTGhY0oM");
        public RefreshToken RefreshToken => new RefreshToken("Eq3OoSyboa06waz932N3qHW/YSVu8nC+nlq3pXy+qEXIrKWOulFURA/0KEnyNM8M+KgjAqiJ5FEApwBWRDum3A==");
    }
}
