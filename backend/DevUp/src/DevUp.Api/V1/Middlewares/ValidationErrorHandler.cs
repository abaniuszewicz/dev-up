using System.Threading.Tasks;
using DevUp.Domain.Seedwork.Exceptions;
using Microsoft.AspNetCore.Http;

namespace DevUp.Api.V1.Middlewares
{
    public sealed class ValidationErrorHandler : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException exception)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { exception.Errors });
            }
        }
    }
}
