using DevUp.Infrastructure.Documentation;
using DevUp.Infrastructure.Logging;
using DevUp.Infrastructure.Postgres;
using DevUp.Infrastructure.Postgres.JwtIdentity;
using DevUp.Infrastructure.Postgres.Migrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddLogger();
            services.AddJwtAuthentication();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwagger();
            services.AddDatabaseMigrator();
            services.AddPostgresInfrastructure();
            services.AddPostgresUserManager();
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

