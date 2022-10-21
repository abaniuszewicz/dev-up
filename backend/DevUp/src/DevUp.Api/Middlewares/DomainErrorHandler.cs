using DevUp.Api.Contracts.V1;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Seedwork.Exceptions;
using Microsoft.AspNetCore.Http;

namespace DevUp.Api.Middlewares
{
    internal sealed class DomainErrorHandler : ErrorHandler
    {
        public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (DomainException exception) when (exception is IIdentityException { CanLeak: false } identityException)
            {
                var response = new ErrorResponse("identity", "Invalid request.");
                var statusCode = StatusCodes.Status400BadRequest;

                await WriteErrorResponse(context, statusCode, response);
            }
            catch (DomainException exception)
            {
                var code = GetErrorCode(exception);
                var response = new ErrorResponse(code, exception.Errors);
                var statusCode = exception switch
                {
                    DomainDataValidationException => StatusCodes.Status400BadRequest,
                    DomainBusinessRuleValidationException => StatusCodes.Status400BadRequest,
                    DomainNotFoundException => StatusCodes.Status404NotFound,
                    _ => StatusCodes.Status400BadRequest
                };

                await WriteErrorResponse(context, statusCode, response);
            }
        }
    }
}
