using AutoMapper;
using DevUp.Api.Contracts.V1;
using DevUp.Api.Contracts.V1.Organization.Requests;
using DevUp.Api.Contracts.V1.Organization.Responses;
using DevUp.Application.Organization.Commands;
using DevUp.Application.Organization.Queries;
using MediatR;

namespace DevUp.Api.Minimal.V1
{
    public class TeamsEndpoints : IEndpoints
    {
        public static IEndpointRouteBuilder DefineEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet(Contracts.Route.Api.V1.Teams.GetAll, GetAllTeams)
                .Produces<IEnumerable<TeamResponse>>(StatusCodes.Status200OK)
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError)
                .WithTags("Teams");

            app.MapGet(Contracts.Route.Api.V1.Teams.GetById, GetTeamById)
                .WithName(nameof(GetTeamById))
                .Produces<TeamResponse>(StatusCodes.Status200OK)
                .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError)
                .WithTags("Teams");

            app.MapPost(Contracts.Route.Api.V1.Teams.Create, Create)
                .Produces(StatusCodes.Status201Created)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError)
                .WithTags("Teams");

            app.MapPut(Contracts.Route.Api.V1.Teams.Update, Update)
                .Produces(StatusCodes.Status200OK)
                .Produces<ErrorResponse>(StatusCodes.Status400BadRequest)
                .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError)
                .WithTags("Teams");

            app.MapDelete(Contracts.Route.Api.V1.Teams.Delete, Delete)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
                .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError)
                .WithTags("Teams");

            return app;
        }

        private static async Task<IResult> GetAllTeams(IMapper mapper, IMediator mediator, CancellationToken cancellationToken)
        {
            var query = new GetAllTeamsQuery();
            var result = await mediator.Send(query, cancellationToken);
            var response = mapper.Map<IEnumerable<TeamResponse>>(result);
            return Results.Ok(response);
        }

        private static async Task<IResult> GetTeamById(Guid teamId, IMapper mapper, IMediator mediator, CancellationToken cancellationToken)
        {
            var query = new GetTeamQuery() { Id = teamId };
            var result = await mediator.Send(query, cancellationToken);
            var response = mapper.Map<TeamResponse>(result);
            return Results.Ok(response);
        }

        private static async Task<IResult> Create(CreateTeamRequest request, IMapper mapper, IMediator mediator, CancellationToken cancellationToken)
        {
            var command = mapper.Map<CreateTeamCommand>(request);
            await mediator.Send(command, cancellationToken);
            return Results.CreatedAtRoute(nameof(GetTeamById), new { TeamId = command.Id }, null);
        }

        private static async Task<IResult> Update(Guid teamId, UpdateTeamRequest request, IMapper mapper, IMediator mediator, CancellationToken cancellationToken)
        {
            var command = mapper.Map<UpdateTeamCommand>(request, opts => opts.AfterMap((_, c) => c.Id = teamId));
            await mediator.Send(command, cancellationToken);
            return Results.Ok();
        }

        private static async Task<IResult> Delete(Guid teamId, IMapper mapper, IMediator mediator, CancellationToken cancellationToken)
        {
            var command = new DeleteTeamCommand() { Id = teamId };
            await mediator.Send(command, cancellationToken);
            return Results.NoContent();
        }
    }
}
