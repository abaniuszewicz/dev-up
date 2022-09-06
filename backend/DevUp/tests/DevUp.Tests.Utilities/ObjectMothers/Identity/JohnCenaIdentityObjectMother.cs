using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Tests.Utilities.ObjectMothers.Identity
{
    internal class JohnCenaIdentityObjectMother : IIdentityObjectMother
    {
        public DeviceId DeviceId => new DeviceId("b691b7a8-b251-4b11-8034-f3a0a154dffe");
        public Device Device => new Device(DeviceId, "IPhone (John)");
        public Username Username => new Username("john-cena");
        public Password Password => new Password("s3cUr3-p4s$");
        public PasswordHash PasswordHash => new PasswordHash("AQAAAAEAACcQAAAAEMlUrqalK2SdV/AZ5rphBFm0/RztUrCgPbvFMbkPwov7QS/nueC9KSf6iOw7JCBQ9g==");
        public UserId UserId => new UserId(Guid.Parse("a5f6b484-9433-4523-904d-822caf103c7b"));
        public User User => new User(UserId, Username);
        public Token Token => new Token("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJzdWIiOiIxNDdkNzM3OC1jMjVmLTRiNDQtYWFkZS03NDA5MzUzOTQ4ODEiLCJqdGkiOiI5NDQ1ODhlMC00MGJmLTRlZDYtYjkxNC1hNWQ3NWI5NGUzY2EiLCJuYmYiOjE2NTU5MTA0MzYsImV4cCI6MTY1NTkxMDczNiwiaWF0IjoxNjU1OTEwNDQwfQ.Szys6gBlYeRxuwermu1hnFdktemKlW5DbFxg1Lr1fcg");
        public RefreshToken RefreshToken => new RefreshToken("JCzXfzfZ/n97d9qQ9z1rvrAeOXikMns8jimyDzpqtg9gMWUrz3OcqqqthxpUG+9WaSFV1LdtWH4x7aDqBJ21gg==");
    }
}
