using DevUp.Domain.Identity.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Infrastructure.Postgres.Identity
{
    public static class PostgresIdentityInstaller
    {
        public static IServiceCollection AddPostgresIdentity(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            return services;
        }
    }
}
