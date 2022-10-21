using DevUp.Infrastructure.Http.Documentation.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace DevUp.Infrastructure.Http.Documentation
{
    internal static class DocumentationInstaller
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
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

            services.ConfigureSwaggerGen(options =>
            {
                var docName = "DevUp.Api.xml";
                var docPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, docName);
                if (!File.Exists(docPath))
                    throw new DocumentationFileNotFoundException(docPath);

                options.IncludeXmlComments(docPath);
            });



            return services;
        }

        public static IApplicationBuilder UseSwaggerDoc(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
    }
}
