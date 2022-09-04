using DevUp.Api.Contracts;
using DevUp.Application;
using DevUp.Domain.Identity;
using DevUp.Infrastructure;
using DevUp.Infrastructure.Http;
using DevUp.Infrastructure.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddApplication();
            services.AddIdentity();
            services.AddInfrastructure();
            services.AddHttpInfrastructure();
            services.AddPostgresInfrastructure();
            services.AddControllers(opt => opt.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())));
            services.AddEndpointsApiExplorer();
            services.AddRouting();
            services.AddContractsDocumentation();
            services.AddAutoMapper(typeof(IApiMarker).Assembly);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(opt => opt.MapControllers());
        }
    }
}

