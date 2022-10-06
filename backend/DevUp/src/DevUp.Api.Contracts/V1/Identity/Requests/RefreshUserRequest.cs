namespace DevUp.Api.Contracts.V1.Identity.Requests
{
    public class RefreshUserRequest
    {
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJzdWIiOiIxNDdkNzM3OC1jMjVmLTRiNDQtYWFkZS03NDA5MzUzOTQ4ODEiLCJqdGkiOiI5NDQ1ODhlMC00MGJmLTRlZDYtYjkxNC1hNWQ3NWI5NGUzY2EiLCJuYmYiOjE2NTU5MTA0MzYsImV4cCI6MTY1NTkxMDczNiwiaWF0IjoxNjU1OTEwNDQwfQ.Szys6gBlYeRxuwermu1hnFdktemKlW5DbFxg1Lr1fcg</example>
        public string Token { get; init; }
        /// <example>JCzXfzfZ/n97d9qQ9z1rvrAeOXikMns8jimyDzpqtg9gMWUrz3OcqqqthxpUG+9WaSFV1LdtWH4x7aDqBJ21gg==</example>
        public string RefreshToken { get; init; }
        public DeviceRequest Device { get; init; }
    }
}
