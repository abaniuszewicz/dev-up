using MediatR;

namespace DevUp.Application.Identity.Commands.Handlers
{
    internal sealed class RefreshUserCommandHandler : IRequestHandler<RefreshUserCommand>
    {
        public Task<Unit> Handle(RefreshUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
