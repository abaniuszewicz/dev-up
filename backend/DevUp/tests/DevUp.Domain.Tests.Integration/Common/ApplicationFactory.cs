using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevUp.Domain.Tests.Integration.Common
{
    public abstract class ApplicationFactory : IDisposable
    {
        protected IHost Host { get; }
        protected IConfiguration Configuration { get; }

        public ApplicationFactory()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Host = new HostBuilder()
                .ConfigureAppConfiguration(c => c.AddConfiguration(Configuration))
                .ConfigureServices(s => ConfigureServices(s))
                .Build();
        }

        protected virtual IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services;
        }

        public virtual void Dispose()
        {
            Host.Dispose();
        }
    }
}
