namespace DevUp.Api.Contracts.V1.Organization.Requests
{
    public class CreateJoinTeamRequest
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
    }
}
