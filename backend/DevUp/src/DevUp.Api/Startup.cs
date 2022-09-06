using DevUp.Application;
using DevUp.Domain;
using DevUp.Infrastructure;
using DevUp.Infrastructure.Http;
using DevUp.Infrastructure.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDomain();
            services.AddApplication();
            services.AddApi();
            services.AddInfrastructure();
            services.AddHttpInfrastructure();
            services.AddPostgresInfrastructure();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseApi();
            app.UseHttpInfrastructure(env);
        }
    }
}

