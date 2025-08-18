using Hangfire;
using Hangfire.PostgreSql;
using ModuleBankApp.API.Services;

namespace ModuleBankApp.API.Extensions;

public static class HangfireServices
{
    public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHangfire(x => x.UsePostgreSqlStorage(options =>
        {
            options.UseNpgsqlConnection(config.GetConnectionString("HangfirePostgreSQL"));
        }));

        services.AddHangfireServer();
        
        services.AddTransient<InterestJobService>();
        
        return services;
    } 
}