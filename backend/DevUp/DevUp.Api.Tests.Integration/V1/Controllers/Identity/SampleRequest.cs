using Bogus;
using DevUp.Api.Contracts.V1.Identity.Requests;

namespace DevUp.Api.Tests.Integration.V1.Controllers.Identity
{
    public class SampleRequest
    {
        private readonly Faker<RegisterUserRequest> _registerGenerator = new Faker<RegisterUserRequest>()
            .RuleFor(r => r.Username, f => $"{f.Person.FirstName}-{f.Person.LastName}".ToLowerInvariant())
            .RuleFor(r => r.Password, f => f.Internet.Password(prefix: "lU1#", length: 8))
            .RuleFor(r => r.Device, (f, u) => new DeviceRequest()
            {
                Id = f.Random.Guid().ToString(),
                Name = $"iPhone ({f.Person.FirstName})"
            });

        public RegisterUserRequest Register { get; private set; }
        public LoginUserRequest Login { get; private set; }

        public SampleRequest()
        {
            Register = _registerGenerator.Generate();
            Login = new LoginUserRequest()
            {
                Username = Register.Username,
                Password = Register.Password,
                Device = Register.Device,
            };
        }
    }
}
