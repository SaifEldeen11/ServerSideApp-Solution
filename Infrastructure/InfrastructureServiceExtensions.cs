using Application.ServiceImplementation;
using Application.ServiceInterfaces;
using Core.Interfaces;
using Core.InterFaces;
using Infrastructure.Data;
using Infrastructure.Data.Contexts;
using Infrastructure.Data.Identity;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database Contexts
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("ApplicationDb"),
                    sqlOptions => sqlOptions
                        .MigrationsAssembly("Infrastructure")
                        .MigrationsHistoryTable("__ApplicationMigrationsHistory")
                ));

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("IdentityDb"),
                    sqlOptions => sqlOptions
                        .MigrationsAssembly("Infrastructure")
                        .MigrationsHistoryTable("__IdentityMigrationsHistory")
                ));

            // Repository Pattern
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ISwimmerRepository, SwimmerRepository>();
            services.AddScoped<IPerformanceRecordRepository, PerformanceRecordRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDataSeeder, DataSeeder>();

            services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]!));

            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<ICacheService, CacheService>();

            return services;
        }
    }
}