using Microsoft.AspNetCore.Mvc.Testing;
using ModuleBankApp.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.PostgreSql;
using Testcontainers.PostgreSql;
using Xunit;

namespace ModuleBankApp.Tests.Integration;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class IntegrationTestApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("testdb")
        .WithUsername("postgres")
        .WithPassword("root")
        .Build();
    
    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                s => s.ServiceType == typeof(DbContextOptions<ModuleBankAppContext>));

            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<ModuleBankAppContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));

            var hangfireDescriptor = services.FirstOrDefault(d => 
                d.ServiceType == typeof(JobStorage));
            if (hangfireDescriptor != null)
            {
                services.Remove(hangfireDescriptor);
            }

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(_dbContainer.GetConnectionString())));
            
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ModuleBankAppContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }
}

// +