using Serilog;

namespace ModuleBankApp.API.Extensions;

public static class SerilogServices
{
    public static  ConfigureHostBuilder  AddSerilogConfiguration(this  ConfigureHostBuilder  host, IConfiguration config)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithCorrelationId()
            .CreateLogger();

        
        host.UseSerilog();
        return host;
    }
}