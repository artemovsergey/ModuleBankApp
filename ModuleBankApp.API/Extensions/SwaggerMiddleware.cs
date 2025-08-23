namespace ModuleBankApp.API.Extensions;

public static class SwaggerMiddleware
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static WebApplication UseSwaggerMiddleware(this WebApplication app)
    {
        //app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/api/swagger.json", "Банковские счета v1");
            c.SwaggerEndpoint("/swagger/events/swagger.json", "События v1");
        });

        return app;
    }
}