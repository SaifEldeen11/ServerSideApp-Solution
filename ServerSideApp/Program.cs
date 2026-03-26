using Application;
using Application.ServiceInterfaces;
using Infrastructure;
using ServerSideApp.CustomMiddleWare;
using ServerSideApp.Extensions;

namespace ServerSideApp
{
    //admin@swimacademy.com
    // Admin@123
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddIdentityAndJwtServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddWebApplicationServices();
            builder.Services.AddSwaggerServices();

            var app = builder.Build();

            // Seed data
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
                await seeder.SeedAllAsync();
            }

            // Configure pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerConfiguration();
            }

            app.UseGlobalExceptionHandler();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}