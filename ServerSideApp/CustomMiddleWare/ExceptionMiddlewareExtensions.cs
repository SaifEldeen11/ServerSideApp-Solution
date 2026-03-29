using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ServerSideApp.CustomMiddleWare
{
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(err => err.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                var (statusCode, message) = exception switch
                {
                    SwimmerNotFoundException ex => (404, ex.Message),
                    CoachNotFoundException ex => (404, ex.Message),
                    TeamNotFoundException ex => (404, ex.Message),
                    UnauthorizedAccessException ex => (401, ex.Message),
                    _ => (500, exception?.InnerException?.Message ?? exception?.Message ?? "An unexpected error occurred.")
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    statusCode,
                    message
                });
            }));

            return app;
        }
    }
}
