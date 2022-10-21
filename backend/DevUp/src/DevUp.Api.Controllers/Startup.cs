using DevUp.Api.Configuration;
using DevUp.Application;
using DevUp.Domain;
using DevUp.Infrastructure;
using DevUp.Infrastructure.Http;
using DevUp.Infrastructure.Postgres;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace DevUp.Api.Controllers
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
            services.AddDomain(Configuration);
            services.AddApplication(Configuration);
            services.AddInfrastructure();
            services.AddHttpInfrastructure();
            services.AddPostgresInfrastructure(Configuration);
            services.AddApi(Configuration);

            services.AddControllers(opt => opt.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpInfrastructure(env);
            app.UseApi();
            app.UseRouting();
            app.UseEndpoints(opt => opt.MapControllers());
        }
    }
}

