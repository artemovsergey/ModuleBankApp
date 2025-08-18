using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace ModuleBankApp.API.Extensions;

public static class HealthMiddleware
{
    public static WebApplication UseHealthMiddleware(this WebApplication app)
    {
        
        // Live и Ready endpoints
        app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false, // только сервис жив
                ResponseWriter = async (context, report) =>
                {
                    var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
        
                    var log = new
                    {
                        Path = context.Request.Path,
                        Method = context.Request.Method,
                        Status = report.Status.ToString(),
                        CorrelationId = correlationId,
                        Timestamp = DateTime.UtcNow
                    };
        
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Health check called {@Log}", log);
        
                    // Отправляем стандартный ответ
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        status = report.Status.ToString()
                    });
                }
            })
            .RequireAuthorization("AllowAll");
        
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
        
                var result = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description
                    })
                };
        
                await context.Response.WriteAsJsonAsync(result);
            }
        }).RequireAuthorization(["AllowAll"]);


        return app;
    }
}