﻿using System.Collections.Generic;
using Bogus;
using DevUp.Api.Contracts.V1.Organization.Requests;

namespace DevUp.Api.Tests.Integration.V1.Organization
{
    public class OrganizationFaker
    {
        private static readonly HashSet<string> AlreadyUsedTeamNames = new();
        
        public Faker Faker { get; }

        public CreateTeamRequest CreateTeamRequest { get; }
        public UpdateTeamRequest UpdateTeamRequest { get; }

        public OrganizationFaker()
        {
            Faker = new Faker();

            CreateTeamRequest = new CreateTeamRequest
            {
                Name = GetTeamName()
            };

            UpdateTeamRequest = new UpdateTeamRequest
            {
                Name = CreateTeamRequest.Name
            };
        }

        private string GetTeamName()
        {
            var teamName = Faker.Company.CompanyName();
            while (AlreadyUsedTeamNames.Contains(teamName))
            {
                var suffixLength = Faker.Random.Byte(1, 3);
                var suffix = Faker.Random.String2(suffixLength);
                teamName = $"{teamName}-{suffix}";
            }

            AlreadyUsedTeamNames.Add(teamName);
            return teamName;
        }
    }
}
