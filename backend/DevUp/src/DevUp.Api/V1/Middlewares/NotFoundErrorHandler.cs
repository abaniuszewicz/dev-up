﻿using System.Threading.Tasks;
using DevUp.Api.Contracts.V1;
using DevUp.Domain.Seedwork.Exceptions;
using Humanizer;
using Microsoft.AspNetCore.Http;

namespace DevUp.Api.V1.Middlewares
{
    internal sealed class NotFoundErrorHandler : IMiddleware
    {

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (DomainNotFoundException exception)
            {
                var code = exception.GetType().Name
                    .Underscore()
                    .ToLowerInvariant()
                    .Replace("_exception", "");
                var error = new ErrorResponse(code, exception.Errors);

                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(error);
            }
        }
    }
}