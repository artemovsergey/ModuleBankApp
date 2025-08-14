namespace ModuleBankApp.Tests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestApplicationFactory>
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;

    protected BaseIntegrationTest(IntegrationTestApplicationFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
    }
}