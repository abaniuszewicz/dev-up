using AutoMapper;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.ValueObjects;
using MediatR;

namespace DevUp.Application.Identity.Commands.Handlers
{
    internal sealed class RefreshUserCommandHandler : IRequestHandler<RefreshUserCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly ITokenStore _tokenStore;

        public RefreshUserCommandHandler(IIdentityService identityService, IMapper mapper, ITokenStore tokenStore)
        {
            _identityService = identityService;
            _mapper = mapper;
            _tokenStore = tokenStore;
        }

        public async Task<Unit> Handle(RefreshUserCommand request, CancellationToken cancellationToken)
        {
            var token = new Token(request.Token);
            var refreshToken = new RefreshToken(request.RefreshToken);
            var device = new Device(new(request.DeviceId), new(request.DeviceName));

            var identityResult = await _identityService.RefreshAsync(token, refreshToken, device, cancellationToken);
            var tokenPair = _mapper.Map<TokenPair>(identityResult);
            _tokenStore.Set(tokenPair);
            return default;
        }
    }
}
