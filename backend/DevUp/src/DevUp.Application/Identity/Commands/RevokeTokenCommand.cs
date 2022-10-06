using MediatR;

namespace DevUp.Application.Identity.Commands
{
    public sealed class RevokeTokenCommand : IRequest
    {
        public string RefreshToken { get; init; }
    }
}
