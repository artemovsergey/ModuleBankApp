using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ModuleBankApp.API.Data;
using ModuleBankApp.API.Infrastructure.Data;
using Xunit;

namespace ModuleBankApp.Tests.Integration;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestApplicationFactory>
{
    protected readonly ISender Sender;
    protected readonly ModuleBankAppContext ModuleBankAppContext;

    protected BaseIntegrationTest(IntegrationTestApplicationFactory factory)
    {
        var scope = factory.Services.CreateScope();
        Sender = scope.ServiceProvider.GetRequiredService<ISender>();
        ModuleBankAppContext = scope.ServiceProvider.GetRequiredService<ModuleBankAppContext>();
    }
}

