using DevUp.Api.Contracts.V1.Identity.Requests;
using DevUp.Api.Contracts.V1.Identity.Responses;
using DevUp.Api.Sdk;
using Refit;

namespace DevUp.Sdk.Sample
{
    public static class Program
    {
        private static readonly DeviceRequest _device = new DeviceRequest()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Name of this device"
        };

        public static async Task Main(string[] args)
        {
            CustomConsole.WriteMessage($"This is a DevUp.Api.Sdk sample consumer.");
            CustomConsole.LineBreak();

            var hostUrl = "https://localhost:65532";
            var identityClient = RestService.For<IIdentityApi>(hostUrl);

            try
            {
                CustomConsole.WriteMessage("First, let's create an user.");
                await Register(identityClient);
                CustomConsole.LineBreak();

                CustomConsole.WriteMessage("Now, let's try to log in. You can either use credentials you just registered with or enter some bonkers data to see if it will fail as expected.");
                await Login(identityClient);
                CustomConsole.LineBreak();

                CustomConsole.WriteMessage("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception exception)
            {
                CustomConsole.WriteError($"Unhandled exception. Is API listening at '{hostUrl}'?");
                CustomConsole.WriteError(exception.Message);
            }
        }

        private static async Task Register(IIdentityApi identityClient)
        {
            CustomConsole.WriteMessage("Please enter username and press enter.");
            var username = CustomConsole.ReadLine();
            CustomConsole.WriteMessage("Please enter password and press enter.");
            var password = CustomConsole.ReadLine();
            CustomConsole.WriteMessage($"Creating user '{username}' with password '{password}'...");

            var request = new RegisterUserRequest() { Username = username, Password = password, Device = _device };
            var response = await identityClient.Register(request, CancellationToken.None);
            DisplayResponse(response);
        }

        private static async Task Login(IIdentityApi identityClient)
        {
            CustomConsole.WriteMessage("Please enter username and press enter.");
            var username = CustomConsole.ReadLine();
            CustomConsole.WriteMessage("Please enter password and press enter.");
            var password = CustomConsole.ReadLine();
            CustomConsole.WriteMessage($"Logging in using username '{username}' and password '{password}'...");

            var request = new LoginUserRequest() { Username = username, Password = password, Device = _device };
            var response = await identityClient.Login(request, CancellationToken.None);
            DisplayResponse(response);
        }

        private static void DisplayResponse(ApiResponse<IdentityResponse> response)
        {
            if (response.IsSuccessStatusCode && response.Content.Success)
            {
                CustomConsole.WriteMessage("Request was successful. Received new token pair.");
                CustomConsole.WriteMessage($"Token: {response.Content.Token}");
                CustomConsole.WriteMessage($"Refresh token: {response.Content.RefreshToken}");
            }
            else
            {
                CustomConsole.WriteError("Request was not successful.");
                CustomConsole.WriteError($"Status code: {(int)response.StatusCode} ({response.StatusCode})");
                var reason = response.Content is not null ? string.Join(Environment.NewLine, response.Content.Errors) : response.Error?.Content;
                CustomConsole.WriteError($"Reason: {reason}");
            }
        }
    }
}