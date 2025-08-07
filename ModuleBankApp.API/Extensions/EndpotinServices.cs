using System.Reflection;

namespace ModuleBankApp.API.Extensions;

public static class EndpointServices
{
    public static IServiceCollection UseEndpoints(this IServiceCollection service)
    {
        // get assembly
        var currentAssembly = Assembly.GetExecutingAssembly();

        // get slices
        var slices = currentAssembly.GetTypes()
            .Where(t => typeof(ISlice).IsAssignableFrom(t) &&
                        t != typeof(ISlice) &&
                        t.IsPublic && !t.IsAbstract);
        //register as singleton
        foreach (var slice in slices)
        {
            service.AddSingleton(typeof(ISlice), slice);
        }

        return service;
    }

    // middleware
    public static IEndpointRouteBuilder MapSliceEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        foreach (ISlice slice in endpointRouteBuilder.ServiceProvider.GetServices<ISlice>())
        {
            slice.AddPoint(endpointRouteBuilder);
        }

        return endpointRouteBuilder;
    }
}

public interface ISlice
{
    void AddPoint(IEndpointRouteBuilder endpoint);
}

public sealed class ExampleEndpoint : ISlice
{
    public void AddPoint(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("", () => "");
    }
}