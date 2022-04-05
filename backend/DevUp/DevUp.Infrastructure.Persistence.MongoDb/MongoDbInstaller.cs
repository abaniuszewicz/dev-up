using DevUp.Infrastructure.Persistence.MongoDb.Repositories;
using DevUp.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DevUp.Infrastructure.Persistence.MongoDb
{
    public static class MongoDbInstaller
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services)
        {
            services.AddSingleton<MongoSettings>();
            services.AddScoped<IMongoClient, MongoClient>(s =>
            {
                var settings = s.GetRequiredService<MongoSettings>();
                return new MongoClient(settings.ConnectionString);
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddAutoMapper(typeof(MongoDbInstaller).Assembly);
            return services;
        }
    }
}
