using DevUp.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DevUp.Infrastructure.Http.Middlewares
{
    internal sealed class InfrastructureErrorHandler : IMiddleware
    {
        private readonly ILogger<InfrastructureErrorHandler> _logger;

        public InfrastructureErrorHandler(ILogger<InfrastructureErrorHandler> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch(InfrastructureException exception)
            {
                _logger.LogError(exception, "Infrastructure error occurred during processing request with trade id {traceId}.", context.TraceIdentifier);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { Errors = new[] { "An internal server error occurred." } });
            }
        }
    }
}
