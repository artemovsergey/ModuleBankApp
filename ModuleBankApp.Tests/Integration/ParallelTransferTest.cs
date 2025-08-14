using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Features.Accounts;
using ModuleBankApp.API.Features.Transactions;
using Xunit;

namespace ModuleBankApp.Tests.Integration;

public class ParallelTransferTests(IntegrationTestApplicationFactory factory)
    : BaseIntegrationTest(factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ParallelTransfers_Should_Preserve_TotalBalance()
    {
        // Arrange
        var accountSender = new Account
        {
            Id = Guid.NewGuid(),
            Type = AccountType.Checking,
            Currency = "USD",
            Balance = 1000,
            OwnerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            CreatedAt = DateTime.UtcNow
        };

        var accountReceiver = new Account
        {
            Id = Guid.NewGuid(),
            Type = AccountType.Checking,
            Currency = "USD",
            Balance = 0,
            OwnerId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        await ModuleBankAppContext.Accounts.AddRangeAsync(accountSender, accountReceiver);
        await ModuleBankAppContext.SaveChangesAsync();

        const decimal transferAmount = 10m;
        const int totalTransfers = 50;

        // Act
        var tasks = Enumerable.Range(0, totalTransfers)
            .Select(async i =>
            {
                var dto = new TransactionDto
                {
                    AccountId = accountSender.Id,
                    CounterPartyAccountId = accountReceiver.Id,
                    Currency = "USD",
                    Amount = transferAmount,
                    Type = TransactionType.Debit,
                    Description = $"Parallel transfer test #{i}"
                };

                var response = await _client.PostAsJsonAsync("/transaction/transfer", dto);
                // testOutputHelper.WriteLine($"Request {i}: {response.StatusCode}");
                // testOutputHelper.WriteLine($"Request {i}: {response.Content.ReadAsStringAsync().Result}");
                return response;
            });

        await Task.WhenAll(tasks);

        // Assert
        var finalSender = await ModuleBankAppContext.Accounts
            .AsNoTracking()
            .FirstAsync(a => a.Id == accountSender.Id);

        var finalReceiver = await ModuleBankAppContext.Accounts
            .AsNoTracking()
            .FirstAsync(a => a.Id == accountReceiver.Id);

        var totalBalance = finalSender.Balance + finalReceiver.Balance;

        totalBalance.Should().Be(accountSender.Balance + accountReceiver.Balance,
            because: "total money in the system must stay the same after concurrent transfers");
    }
}

// +