using AutoMapper;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.ValueObjects;
using MediatR;

namespace DevUp.Application.Identity.Commands.Handlers
{
    internal sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand>
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly ITokenStore _tokenStore;

        public LoginUserCommandHandler(IIdentityService identityService, IMapper mapper, ITokenStore tokenStore)
        {
            _identityService = identityService;
            _mapper = mapper;
            _tokenStore = tokenStore;
        }

        public async Task<Unit> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var username = new Username(request.Username);
            var password = new Password(request.Password);
            var device = new Device(new(request.DeviceId), new(request.DeviceName));

            var identityResult = await _identityService.LoginAsync(username, password, device, cancellationToken);
            var tokenPair = _mapper.Map<TokenPair>(identityResult);
            _tokenStore.Set(tokenPair);
            return default;
        }
    }
}
