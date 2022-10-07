using System.Threading.Tasks;
using DevUp.Api.Contracts.V1;
using DevUp.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DevUp.Api.V1.Middlewares
{
    internal sealed class InfrastructureErrorHandler : ErrorHandler
    {
        private readonly ILogger<InfrastructureErrorHandler> _logger;

        public InfrastructureErrorHandler(ILogger<InfrastructureErrorHandler> logger)
        {
            _logger = logger;
        }

        public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (InfrastructureException exception)
            {
                _logger.LogError(exception, "Infrastructure error occurred during processing request with trade id {traceId}. Reason: {reason}", context.TraceIdentifier, exception.Message);

                var response = new ErrorResponse("infrastructure", "An internal server error occurred.");
                var statusCode = StatusCodes.Status500InternalServerError;

                await WriteErrorResponse(context, statusCode, response);
            }
        }
    }
}
