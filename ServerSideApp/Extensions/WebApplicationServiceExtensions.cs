namespace ServerSideApp.Extensions
{
    public static class WebApplicationServiceExtensions
    {
        public static IServiceCollection AddWebApplicationServices(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler =
                        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition =
                        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            services.AddEndpointsApiExplorer();

            // CORS Configuration
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Add HTTP Context Accessor
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
