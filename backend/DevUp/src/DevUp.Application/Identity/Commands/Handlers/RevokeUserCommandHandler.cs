using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.ValueObjects;
using MediatR;

namespace DevUp.Application.Identity.Commands.Handlers
{
    internal sealed class RevokeUserCommandHandler : IRequestHandler<RevokeTokenCommand>
    {
        private readonly ITokenService _tokenService;

        public RevokeUserCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = new RefreshToken(request.RefreshToken);
            await _tokenService.RevokeAsync(refreshToken, cancellationToken);
            return default;
        }
    }
}
