using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Api.Contracts
{
    public static class SwaggerInstaller
    {
        public static IServiceCollection AddContractsDocumentation(this IServiceCollection services)
        {
            services.ConfigureSwaggerGen(options =>
            {
                var docName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var docPath = Path.Combine(AppContext.BaseDirectory, docName);
                if (File.Exists(docPath))
                    options.IncludeXmlComments(docPath);
            });

            return services;
        }
    }
}
