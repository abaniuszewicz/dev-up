using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.ValueObjects;
using MediatR;

namespace DevUp.Application.Identity.Commands.Handlers
{
    internal sealed class RefreshUserCommandHandler : IRequestHandler<RefreshUserCommand>
    {
        private readonly IIdentityService _identityService;

        public RefreshUserCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Unit> Handle(RefreshUserCommand request, CancellationToken cancellationToken)
        {
            var token = new Token(request.Token);
            var refreshToken = new RefreshToken(request.RefreshToken);
            var device = new Device(new DeviceId(request.DeviceId), request.DeviceName);

            await _identityService.RefreshAsync(token, refreshToken, device, cancellationToken);
            return default;
        }
    }
}
