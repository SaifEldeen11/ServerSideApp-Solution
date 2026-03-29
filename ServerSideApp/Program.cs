using Application;
using Application.ServiceInterfaces;
using Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using ServerSideApp.CustomMiddleWare;
using ServerSideApp.Extensions;
using System.Threading.RateLimiting;

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


            builder.Services.AddRateLimiter(ratelimiterOptions =>
            {
                ratelimiterOptions.OnRejected = async (context,cancellationToken) =>
                {  
                    context.HttpContext.Response.StatusCode = 429;
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        statusCode = 429,
                        message = "Too many requests. Please try again later."
                    }, cancellationToken);
                };


                ratelimiterOptions.AddFixedWindowLimiter("general", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.PermitLimit = 30;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });

                ratelimiterOptions.AddFixedWindowLimiter("auth", opt =>
                {
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.PermitLimit = 10;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0;
                });
                ratelimiterOptions.RejectionStatusCode = 429;
            });

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
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}