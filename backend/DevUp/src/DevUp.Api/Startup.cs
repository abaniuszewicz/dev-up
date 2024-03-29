﻿using DevUp.Application;
using DevUp.Domain;
using DevUp.Infrastructure;
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
            services.AddDomain(Configuration);
            services.AddApplication();
            services.AddApi();
            services.AddInfrastructure(Configuration);
            services.AddPostgresInfrastructure(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpInfrastructure(env);
            app.UseApi();
        }
    }
}

