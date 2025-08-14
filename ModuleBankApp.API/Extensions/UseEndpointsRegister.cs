using System.Reflection;

namespace ModuleBankApp.API.Extensions;

public static class EndpointsRegister
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static WebApplication UseEndpointsRegister(this WebApplication app)
    {
        var endpointTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && t.Name.EndsWith("Endpoint"));

        foreach (var type in endpointTypes)
        {
            var method = type.GetMethod("MapEndpoint", [typeof(WebApplication)]);
            method?.Invoke(null, [app]);
        }

        return app;
    }
}

// +