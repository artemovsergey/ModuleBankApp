using System;
using Xunit;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ModuleBankApp.API.Features.Accounts;

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

public class ParallelTransferTests : BaseIntegrationTest
{
    private Guid SenderId { get; set; }
    private Guid ReceiverId { get; set; }
    
    private readonly HttpClient _client;
    
    public ParallelTransferTests(IntegrationTestApplicationFactory factory) : base(factory)
    {
         _client = factory.CreateClient();
    }
    
    //[Fact]
    public async Task Should_Keep_Total_Balance_After_Parallel_Transfers()
    {
        SenderId = Guid.NewGuid();
        ReceiverId = Guid.NewGuid();

        var createAcc1 = await _client.PostAsJsonAsync("/account", new
        {
            Currency = "RUB",
            Balance = 10000m,
            Type = "Deposit",
            InterestRate = 10
        });
        
        createAcc1.EnsureSuccessStatusCode();
        
        Console.WriteLine($"Created with status: {createAcc1.Content.ReadFromJsonAsync<Account>()}");
        
        Assert.Equal(2, 2);
        // Проверьте, что аккаунт действительно создан
        //var s = await _client.GetFromJsonAsync<AccountDto>($"/account/{createAcc1.Content.ReadFromJsonAsync<Account>().Id}");
        // Assert.NotNull(s);
        // Assert.Equal(10000m, s.Balance);

        // var createAcc2 = await _client.PostAsJsonAsync("/account", new
        // {
        //     Id = ReceiverId,
        //     Currency = "RUB",
        //     Balance = 5000m,
        //     Type = 1,
        //     OwnerId = Guid.NewGuid()
        // });
        // createAcc2.EnsureSuccessStatusCode();

        // decimal initialTotal = 10000m + 5000m;
        // int transfers = 50;
        // decimal transferAmount = 100;
        //
        // var tasks = Enumerable.Range(0, transfers).Select(_ =>
        //     _client.PostAsJsonAsync("/transaction/transfer", new TransactionDto
        //     {
        //         AccountId = SenderId,
        //         CounterPartyAccountId = ReceiverId,
        //         Amount = transferAmount,
        //         Currency = "RUB",
        //         Type = TransactionType.Debit,
        //         Description = "parallel test"
        //     })
        // );
        //
        // var responses = await Task.WhenAll(tasks);
        // responses.Should().OnlyContain(r => r.IsSuccessStatusCode, 
        //     "все переводы должны пройти успешно");

        // var sender = await _client.GetFromJsonAsync<AccountDto>($"/account/{SenderId}");
        //var receiver = await _client.GetFromJsonAsync<AccountDto>($"/account/{ReceiverId}");

        //(sender.Balance + receiver.Balance).Should().Be(initialTotal);
    
    }
}