using DevUp.Api.Contracts.V1;
using Humanizer;
using Microsoft.AspNetCore.Http;

namespace DevUp.Api.Middlewares
{
    internal abstract class ErrorHandler : IMiddleware
    {
        public abstract Task InvokeAsync(HttpContext context, RequestDelegate next);

        protected static async Task WriteErrorResponse(HttpContext context, int statusCode, ErrorResponse response)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response);
        }

        protected static string GetErrorCode<TException>(TException exception)
            where TException : Exception
        {
            return exception.GetType().Name
                .Underscore()
                .ToLowerInvariant()
                .Replace("_exception", "");
        }
    }
}
