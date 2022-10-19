using MediatR;

namespace DevUp.Application.Identity.Commands
{
    public sealed class LoginUserCommand : IRequest
    {
        public string Username { get; init; }
        public string Password { get; init; }
        public Guid DeviceId { get; init; }
        public string DeviceName { get; init; }
    }
}
