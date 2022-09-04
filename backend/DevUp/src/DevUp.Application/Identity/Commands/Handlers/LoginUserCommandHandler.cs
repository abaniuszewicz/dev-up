using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.ValueObjects;
using MediatR;

namespace DevUp.Application.Identity.Commands.Handlers
{
    internal class LoginUserCommandHandler : IRequestHandler<LoginUserCommand>
    {
        private readonly IIdentityService _identityService;

        public LoginUserCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Unit> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var username = new Username(request.Username);
            var password = new Password(request.Password);
            var device = new Device(new DeviceId(request.DeviceId), request.DeviceName);

            await _identityService.LoginAsync(username, password, device, cancellationToken);
            return default;
        }
    }
}
