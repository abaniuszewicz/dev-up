using AutoMapper;
using DevUp.Api.Contracts.V1;
using DevUp.Api.Contracts.V1.Identity.Requests;
using DevUp.Api.Contracts.V1.Identity.Responses;
using DevUp.Application.Identity;
using DevUp.Application.Identity.Commands;
using MediatR;

namespace DevUp.Api.Minimal.V1
{
    public class IdentityEndpoints : IEndpoints
    {
        public static IEndpointRouteBuilder DefineEndpoints(IEndpointRouteBuilder app)
        {
            app.MapPost(Contracts.Route.Api.V1.Identity.Register, Register)
                .Produces<IdentityResponse>(StatusCodes.Status200OK)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError)
                .WithTags("Identity");

            app.MapPost(Contracts.Route.Api.V1.Identity.Login, Login)
                .Produces<IdentityResponse>(StatusCodes.Status200OK)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError)
                .WithTags("Identity");

            app.MapPost(Contracts.Route.Api.V1.Identity.Refresh, Refresh)
                .Produces<IdentityResponse>(StatusCodes.Status200OK)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError)
                .WithTags("Identity");

            var x = app.MapPost(Contracts.Route.Api.V1.Identity.Revoke, Revoke)
                .Produces<IdentityResponse>(StatusCodes.Status200OK)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError)
                .WithTags("Identity");

            return app;
        }

        private static async Task<IResult> Register(RegisterUserRequest request, IMapper mapper, IMediator mediator, ITokenStore tokenStore, CancellationToken cancellationToken)
        {
            var command = mapper.Map<RegisterUserCommand>(request);
            await mediator.Send(command, cancellationToken);

            var tokenPair = tokenStore.Get();
            var response = mapper.Map<IdentityResponse>(tokenPair);
            return Results.Ok(response);
        }

        private static async Task<IResult> Login(LoginUserRequest request, IMapper mapper, IMediator mediator, ITokenStore tokenStore, CancellationToken cancellationToken)
        {
            var command = mapper.Map<LoginUserCommand>(request);
            await mediator.Send(command, cancellationToken);

            var tokenPair = tokenStore.Get();
            var response = mapper.Map<IdentityResponse>(tokenPair);
            return Results.Ok(response);
        }

        private static async Task<IResult> Refresh(RefreshUserRequest request, IMapper mapper, IMediator mediator, ITokenStore tokenStore, CancellationToken cancellationToken)
        {
            var command = mapper.Map<RefreshUserCommand>(request);
            await mediator.Send(command, cancellationToken);

            var tokenPair = tokenStore.Get();
            var response = mapper.Map<IdentityResponse>(tokenPair);
            return Results.Ok(response);
        }

        private static async Task<IResult> Revoke(RevokeTokenRequest request, IMapper mapper, IMediator mediator, ITokenStore tokenStore, CancellationToken cancellationToken)
        {
            var command = mapper.Map<RevokeTokenCommand>(request);
            await mediator.Send(command, cancellationToken);
            return Results.Ok();
        }
    }
}
