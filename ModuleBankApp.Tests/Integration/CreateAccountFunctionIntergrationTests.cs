using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Features.Accounts.CreateAccount;
using ModuleBankApp.API.Generic;
using Xunit;

namespace ModuleBankApp.Tests.Integration;

public class CreateAccountIntegrationTests : BaseIntegrationTest
{
    private readonly ClaimsPrincipal _userWithValidId = new(new ClaimsIdentity(
        [new Claim(ClaimTypes.NameIdentifier, "a191ee39-08a7-4ffa-8f53-3c5f0f5f9b1c")]));

    private readonly ClaimsPrincipal _userWithoutId = new(new ClaimsIdentity());

    private readonly CreateAccountDto _validAccountDto = new(
        AccountType.Checking,
        "USD",
        1000m,
        null
    );

    public CreateAccountIntegrationTests(IntegrationTestApplicationFactory factory) 
        : base(factory) { }

    [Fact]
    public async Task HandleRequest_WithInvalidUser_ReturnsUnauthorized()
    {
        // Act
        var result = await CreateAccountEndpoint.HandleEndpoint(
            _validAccountDto,
            Mediator,    // <- теперь настоящий Mediator из DI
            _userWithoutId);
        
        // Assert
        Assert.IsType<UnauthorizedHttpResult>(result);
    }

    [Fact]
    public async Task HandleRequest_WhenMediatorReturnsFailure_ReturnsBadRequest()
    {
        // Здесь мы подсовываем невалидный DTO
        var invalidDto = new CreateAccountDto(
            AccountType.Deposit, // для депозита ставка обязательна
            "USD",
            1000m,
            null);
    
        var result = await CreateAccountEndpoint.HandleEndpoint(
            invalidDto,
            Mediator,
            _userWithValidId);
    
        var badRequest = Assert.IsType<BadRequest<MbResult<Account>>>(result);
        Assert.False(badRequest.Value.IsSuccess);
        // Assert.Contains("Interest rate is required", badRequest.Value.Error);
    }
    
    [Fact]
    public async Task Handle_ShouldCreateAccount_WhenRequestIsValid()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var request = new CreateAccountRequest(_validAccountDto, ownerId);
    
        // Act
        var result = await Sender.Send(request);
    
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    
        var account = result.Value;
        Assert.Equal(AccountType.Checking, account.Type);
        Assert.Equal("USD", account.Currency);
        Assert.Equal(1000m, account.Balance);
        Assert.Equal(ownerId, account.OwnerId);
    
        // Проверим, что запись реально появилась в БД
        var dbAccount = await ModuleBankAppContext.Accounts.FindAsync(account.Id);
        Assert.NotNull(dbAccount);
        Assert.Equal("USD", dbAccount!.Currency);
    }
}


