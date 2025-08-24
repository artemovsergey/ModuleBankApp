using System.Reflection;

namespace ModuleBankApp.API.Extensions;

// ReSharper disable once UnusedType.Global
public static class EndpointServices
{
    // ReSharper disable once UnusedMember.Global
    public static IServiceCollection UseEndpoints(this IServiceCollection service)
    {
        // get assembly
        var currentAssembly = Assembly.GetExecutingAssembly();

        // get slices
        var slices = currentAssembly.GetTypes()
            .Where(t => typeof(ISlice).IsAssignableFrom(t) &&
                        t != typeof(ISlice) &&
                        t is { IsPublic: true, IsAbstract: false });
        //register as singleton
        foreach (var slice in slices)
        {
            service.AddSingleton(typeof(ISlice), slice);
        }

        return service;
    }

    // middleware
    // ReSharper disable once UnusedMember.Global
    public static IEndpointRouteBuilder MapSliceEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        foreach (var slice in endpointRouteBuilder.ServiceProvider.GetServices<ISlice>())
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

// ReSharper disable once UnusedType.Global
public sealed class ExampleEndpoint : ISlice
{
    public void AddPoint(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapGet("", () => "");
    }
}

