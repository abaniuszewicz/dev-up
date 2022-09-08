using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Bogus;
using DevUp.Api.Contracts.V1.Identity.Requests;

namespace DevUp.Api.Tests.Integration.V1.Identity
{
    public class IdentityFaker
    {
        private static readonly HashSet<string> AlreadyUsedUsernames = new HashSet<string>();
        private static readonly string[] Models = new[] { "iPhone", "Motorola", "PC", "iMac Pro" };
        private static readonly string[] Formats = new[] { "{model} ({name})", "{model} - {name}", "{name}'s {model}", "{name}", "{model}" };

        public Faker Faker { get; }

        public RegisterUserRequest RegisterUserRequest { get; }
        public LoginUserRequest LoginUserRequest { get; }

        public IdentityFaker()
        {
            Faker = new Faker();

            var passwordLength = Faker.Random.Byte(8, 15);
            RegisterUserRequest = new RegisterUserRequest()
            {
                Username = GetUsername(),
                Password = Faker.Internet.Password(passwordLength, prefix: "lowUPP1$"),
                Device = new DeviceRequest()
                {
                    Id = Faker.Random.Guid().ToString(),
                    Name = RandomDeviceName()
                }
            };

            LoginUserRequest = new LoginUserRequest()
            {
                Username = RegisterUserRequest.Username,
                Password = RegisterUserRequest.Password,
                Device = new DeviceRequest()
                {
                    Id = RegisterUserRequest.Device.Id,
                    Name = RegisterUserRequest.Device.Name
                },
            };
        }

        private string GetUsername()
        {
            var username = Regex.Replace($"{Faker.Person.FirstName}-{Faker.Person.LastName}", @"[^a-z\-]", string.Empty).ToLowerInvariant();
            return AlreadyUsedUsernames.Contains(username) 
                ? GetUsername() 
                : username;
        }

        private string RandomDeviceName()
        {
            var model = Faker.PickRandom(Models);
            var format = Faker.PickRandom(Formats);
            return format.Replace("{model}", model).Replace("{name}", Faker.Person.FirstName);
        }
    }
}
