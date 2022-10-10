using AutoMapper;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.ValueObjects;
using MediatR;

namespace DevUp.Application.Identity.Commands.Handlers
{
    internal sealed class RefreshUserCommandHandler : IRequestHandler<RefreshUserCommand>
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ITokenStore _tokenStore;

        public RefreshUserCommandHandler(ITokenService tokenService, IMapper mapper, ITokenStore tokenStore)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _tokenStore = tokenStore;
        }

        public async Task<Unit> Handle(RefreshUserCommand request, CancellationToken cancellationToken)
        {
            var token = new Token(request.Token);
            var refreshToken = new RefreshToken(request.RefreshToken);
            var device = new Device(new DeviceId(request.DeviceId), request.DeviceName);

            var tokenPair = await _tokenService.RefreshAsync(new(token, refreshToken), device, cancellationToken);
            var tokenPairToStore = _mapper.Map<TokenPair>(tokenPair);
            _tokenStore.Set(tokenPairToStore);
            return default;
        }
    }
}
