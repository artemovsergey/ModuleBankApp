using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

namespace ModuleBankApp.API.Extensions;

public static class SerializeServices
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddSerializeServices(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.WriteIndented = true;
        });
      
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
      
        return services;
    }    
}

// +