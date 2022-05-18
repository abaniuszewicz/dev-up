using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DevUp.Infrastructure.Documentation
{
    public static class SwaggerInstaller
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                var docName = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var docPath = Path.Combine(AppContext.BaseDirectory, docName);
                options.IncludeXmlComments(docPath);

                options.AddSecurityDefinition("Bearer", new()
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the bearer scheme",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            Reference = new()
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });

            return services;
        }
    }
}
