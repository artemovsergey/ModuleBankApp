using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Features.Accounts;
using ModuleBankApp.API.Features.Accounts.CreateAccount;
using Xunit;

namespace ModuleBankApp.Tests.Integration;

public class CreateAccountFunctionIntegrationTests(IntegrationTestApplicationFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_ShouldPersistAccount_WhenRequestIsValid()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        var accountDto = new CreateAccountDto(
            Type: AccountType.Deposit,
            Currency: "RUB",
            Balance: 500m,
            InterestRate: 10
        );

        var request = new CreateAccountRequest(accountDto, ownerId);

        // Act
        var result = await Sender.Send(request);

        // Assert
        Assert.True(result.IsSuccess, "Account creation should succeed.");
        Assert.NotNull(result.Value);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal(AccountType.Deposit, result.Value.Type);
        Assert.Equal("RUB", result.Value.Currency);
        Assert.Equal(500m, result.Value.Balance);
        Assert.Equal(ownerId, result.Value.OwnerId);

        // Дополнительно — проверим, что запись реально в базе
        var accountFromDb = await ModuleBankAppContext.Accounts.FindAsync(result.Value.Id);
        Assert.NotNull(accountFromDb);
        Assert.Equal("RUB", accountFromDb.Currency);
        Assert.Equal(500m, accountFromDb.Balance);
    }
    
    [Fact]
    public async Task CreateAccount_WithValidData_ShouldCreateAccountInDatabase()
    {
        // Arrange
        var validRequest = new CreateAccountRequest(
            new CreateAccountDto(
                Type: AccountType.Deposit,
                Currency: "RUB",
                Balance: 500m,
                InterestRate: 10
            ),
            Guid.Parse("a191ee39-08a7-4ffa-8f53-3c5f0f5f9b1c"));

        // Act
        var result = await Sender.Send(validRequest);

        // Assert
        Assert.True(result.IsSuccess);
        
        var createdAccount = await ModuleBankAppContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == result.Value.Id);
        
        Assert.NotNull(createdAccount);
        Assert.Equal(AccountType.Deposit, createdAccount.Type);
        Assert.Equal("RUB", createdAccount.Currency);
        Assert.Equal(500m, createdAccount.Balance);
        Assert.Equal(validRequest.ClaimsId, createdAccount.OwnerId);
    }

    [Fact]
    public async Task CreateAccount_WithInvalidCurrency_ShouldReturnValidationError()
    {
        // Arrange
        var invalidRequest = new CreateAccountRequest(
            new CreateAccountDto(
                Type: AccountType.Deposit,
                Currency: "ROB", // invalid currency code
                Balance: 500m,
                InterestRate: 10
            ),
            Guid.Parse("a191ee39-08a7-4ffa-8f53-3c5f0f5f9b1c"));

        // Act
        var result = await Sender.Send(invalidRequest);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid currency code", result.Error);
    }

    [Fact]
    public async Task CreateAccount_WithNegativeBalanceForChecking_ShouldReturnValidationError()
    {
        // Arrange
        var invalidRequest = new CreateAccountRequest(
            new CreateAccountDto(
                Type: AccountType.Deposit,
                Currency: "RUB",
                Balance: -500m, // negative balance
                InterestRate: 10
            ),
            Guid.Parse("a191ee39-08a7-4ffa-8f53-3c5f0f5f9b1c"));

        // Act
        var result = await Sender.Send(invalidRequest);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Balance cannot be negative", result.Error);
    }

    [Fact]
    public async Task CreateAccount_ForDepositWithoutInterestRate_ShouldReturnValidationError()
    {
        // Arrange
        var invalidRequest = new CreateAccountRequest(
            new CreateAccountDto(
                Type: AccountType.Deposit,  
                Currency: "RUB",
                Balance: 500m,
                InterestRate: 0 // not interest rate
            ),
            Guid.Parse("a191ee39-08a7-4ffa-8f53-3c5f0f5f9b1c"));

        // Act
        var result = await Sender.Send(invalidRequest);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Для вклада должна быть указана процентная ставка", result.Error);
    }
}

// +