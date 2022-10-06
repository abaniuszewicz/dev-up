using System.Threading.Tasks;
using DevUp.Api.Contracts.V1;
using DevUp.Application.Exceptions;
using DevUp.Application.PipelineBehaviors.Exceptions;
using Microsoft.AspNetCore.Http;

namespace DevUp.Api.V1.Middlewares
{
    internal sealed class ApplicationErrorHandler : ErrorHandler
    {
        public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ApplicationException exception)
            {
                var code = GetErrorCode(exception);
                var response = new ErrorResponse(code, exception.Errors);
                var statusCode = exception switch
                {
                    ApplicationValidationException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status400BadRequest
                };

                await WriteErrorResponse(context, statusCode, response);
            }
        }
    }
}
