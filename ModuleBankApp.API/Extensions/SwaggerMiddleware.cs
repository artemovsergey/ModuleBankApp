namespace ModuleBankApp.API.Extensions;

public static class SwaggerMiddleware
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static WebApplication UseSwaggerMiddleware(this WebApplication app)
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Банковские счета v1");
        });

        return app;
    }
}

// +