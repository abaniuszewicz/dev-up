using System;
using System.Text.RegularExpressions;
using Bogus;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Tests.Unit.Identity
{
    internal class IdentityFaker : Faker<UserId>
    {
        private static readonly string[] Models = new[] { "iPhone", "Motorola", "PC", "iMac Pro" };
        private static readonly string[] Formats = new[] { "{model} ({name})", "{model} - {name}", "{name}'s {model}", "{name}", "{model}" };

        public Faker Faker { get; }

        public UserId UserId { get; } 
        public Username Username { get; }
        public Password Password { get; }
        public PasswordHash PasswordHash { get; }
        public User User { get; }
        public DeviceId DeviceId { get; }
        public Device Device { get; }
        public Token Token { get; }
        public TokenInfo TokenInfo { get; }
        public RefreshToken RefreshToken { get; }
        public RefreshTokenInfo RefreshTokenInfo { get; }

        public IdentityFaker()
        {
            Faker = new Faker();
            var now = Faker.Date.Soon();

            UserId = new UserId(Faker.Random.Guid());
            Username = new Username(Faker.Person.UserName);
            Password = new Password(Faker.Internet.Password());
            PasswordHash = new PasswordHash(Faker.Random.Hash());
            User = new User(UserId, Username);
            DeviceId = new DeviceId(Faker.Random.Guid().ToString());
            Device = new Device(DeviceId, RandomDeviceName());
            Token = new Token(Regex.Replace("{header}.{payload}.{signature}", "{.+?}", _ => RandomString()));
            TokenInfo = new TokenInfo(Token, Faker.Random.Guid().ToString(), UserId, DeviceId, RandomDateRange(now, 1, now));
            RefreshToken = new RefreshToken(RandomString());
            RefreshTokenInfo = new RefreshTokenInfo(RefreshToken, TokenInfo.Jti, UserId, DeviceId, RandomDateRange(now, 5, TokenInfo.Lifespan.End));
        }

        private DateTimeRange RandomDateRange(DateTime now, int days, DateTime refDate)
        {
            var start = now;
            var end = Faker.Date.Soon(days, refDate);
            return new DateTimeRange(start, end);
        }

        private string RandomString()
        {
            var length = Faker.Random.Byte(5, 255);
            var characters = "1234567890!@#$%^&*()_+abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return Faker.Random.String2(length, characters);
        }

        private string RandomDeviceName()
        {
            var model = Faker.PickRandom(Models);
            var format = Faker.PickRandom(Formats);
            return format.Replace("{model}", model).Replace("{name}", Faker.Person.FirstName);
        }
    }
}
