using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.PostgreSql;
using ModuleBankApp.API.Infrastructure.Data;
using ModuleBankApp.API.Infrastructure.Messaging.Options;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace ModuleBankApp.Tests.Integration;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class IntegrationTestApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithName("postgres_container_name")
        .WithDatabase("testdb")
        .WithUsername("postgres")
        .WithPassword("root")
        .Build();

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:3.12-management")
        .WithName("rabbitmq_container_name")
        .WithHostname("rabbitmq_test")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _rabbitMqContainer.StopAsync();
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

            services.Configure<EventBusOptions>(o =>
            {
                o.HostName = _rabbitMqContainer.Hostname;
                    //o.Port = _rabbitMqContainer.GetMappedPublicPort(5672);
                o.VirtualHost = "/";
                o.ExchangeName = "account.events";
                o.UserName = "guest";
                o.Password = "guest";
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ModuleBankAppContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }
}