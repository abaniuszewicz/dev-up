using DevUp.Domain.Identity.Repositories;
using DevUp.Infrastructure.Postgres.Identity.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Infrastructure.Postgres.Identity
{
    internal static class PostgresIdentityInstaller
    {
        public static IServiceCollection AddPostgresIdentity(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<IDeviceRepository, DeviceRepository>();
            return services;
        }
    }
}
