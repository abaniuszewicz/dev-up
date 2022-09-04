using MediatR;

namespace DevUp.Application.Identity.Commands.Handlers
{
    internal class LoginUserCommandHandler : IRequestHandler<LoginUserCommand>
    {
        public Task<Unit> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
