﻿using System.Threading.Tasks;
using DevUp.Api.Contracts.V1;
using DevUp.Domain.Seedwork.Exceptions;
using Microsoft.AspNetCore.Http;

namespace DevUp.Api.V1.Middlewares
{
    internal sealed class ValidationErrorHandler : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (DomainValidationException exception)
            {
                var code = exception.GetType().Name
                    .ToLowerInvariant()
                    .Replace("exception", "");
                var error = new ErrorResponse(code, exception.Errors);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(error);
            }
        }
    }
}
