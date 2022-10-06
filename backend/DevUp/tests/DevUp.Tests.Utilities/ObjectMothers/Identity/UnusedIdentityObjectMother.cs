using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Tests.Utilities.ObjectMothers.Identity
{
    internal class UnusedIdentityObjectMother : IIdentityObjectMother
    {
        public DeviceId DeviceId => new DeviceId("80461e35-a39b-47dd-a69b-a2f03efe26a6");
        public Device Device => new Device(DeviceId, "unused-device");
        public UserId UserId => new UserId(Guid.Parse("7176ee61-77bf-47d6-bbdb-e2a12663b4ce"));
        public Username Username => new Username("unused-username");
        public Password Password => new Password("unus3D-p4$sw0rd");
        public PasswordHash PasswordHash => new PasswordHash("AQAAAAEAACcQAAAAEBs8HPFycFUJ70OhPIBFtN3vW94uIippQlVkdXv14xxmcDgKK0CiFLvIAfGV8KX+9g==");
        public User User => new User(UserId, Username);
        public Token Token => new Token("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJzdWIiOiI3MTc2ZWU2MS03N2JmLTQ3ZDYtYmJkYi1lMmExMjY2M2I0Y2UiLCJqdGkiOiI3YjdkNWQ1NS1kODNlLTQ0NjctYTIwMi1jNWEzMTk4M2QxOGQiLCJuYmYiOjE2NTU5MDUwNjMsImV4cCI6MTY1NTkwNTM2MywiaWF0IjoxNjU1OTA1MDg5fQ.PYZJ1PrJLrFEINbOMspZifjBCrOs5R8l61NbGy5bgv0");
        public RefreshToken RefreshToken => new RefreshToken("aGnb6xCDG3Ur2mDLkk67i2cDfjS6Xc7bPp3TKirTexbkwL3w9g/N8aVRfJNamr/v/owKtuXZG1+Mm0zhTBbzvw==");
    }
}
