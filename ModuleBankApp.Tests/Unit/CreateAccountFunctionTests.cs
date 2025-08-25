using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using ModuleBankApp.API.Domen;
using Moq;
using ModuleBankApp.API.Features.Accounts.CreateAccount;
using ModuleBankApp.API.Generic;
using ModuleBankApp.API.Infrastructure.Data;
using ModuleBankApp.API.Infrastructure.Data.Interfaces;
using Xunit;

namespace ModuleBankApp.Tests.Unit;

public class CreateAccountFunctionTests
{
    private readonly Mock<IMediator> _mediatorMock = new();

    private readonly ClaimsPrincipal _userWithValidId = new(new ClaimsIdentity(
        [new Claim(ClaimTypes.NameIdentifier, "a191ee39-08a7-4ffa-8f53-3c5f0f5f9b1c")]));

    private readonly ClaimsPrincipal _userWithoutId = new(new ClaimsIdentity());

    private readonly CreateAccountDto _validAccountDto = new(
        AccountType.Checking,
        "USD",
        1000m,
        null
    );
    
    [Fact]
    public async Task HandleRequest_WithInvalidUser_ReturnsUnauthorized()
    {
        // Act
        var result = await CreateAccountEndpoint.HandleEndpoint(
            _validAccountDto,
            _mediatorMock.Object,
            _userWithoutId);

        // Assert
        Assert.IsType<UnauthorizedHttpResult>(result);
    }

    [Fact]
    public async Task HandleRequest_WhenMediatorReturnsFailure_ReturnsBadRequest()
    {
        // Arrange
        var errorResponse = MbResult<Account>.Failure("Validation error");
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateAccountRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(errorResponse);

        // Act
        var result = await CreateAccountEndpoint.HandleEndpoint(
            _validAccountDto,
            _mediatorMock.Object,
            _userWithValidId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<MbResult<Account>>>(result);
        Assert.Equal(errorResponse, badRequestResult.Value);
    }

    [Fact]
    public async Task Handle_ShouldCreateAccount_WhenRequestIsValid()
    {
        // Arrange
        var mockRepo = new Mock<IAccountRepository>();
        var mockLogger = new Mock<ILogger<CreateAccountHandler>>();

        var account = new Account
        {
            Id = Guid.NewGuid(),
            Type = AccountType.Checking,
            Currency = "USD",
            Balance = 1000m,
            CreatedAt = DateTime.UtcNow,
            OwnerId = Guid.NewGuid()
        };

        mockRepo.Setup(r => r.CreateAccount(It.IsAny<Account>()))
            .ReturnsAsync(account);

        var options = new DbContextOptionsBuilder<ModuleBankAppContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var dbContext = new ModuleBankAppContext(options);

        var handler = new CreateAccountHandler(
            mockRepo.Object,
            mockLogger.Object,
            dbContext
        );

        var request = new CreateAccountRequest(_validAccountDto, account.OwnerId);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(account.Id, result.Value.Id);
        Assert.Equal(AccountType.Checking, result.Value.Type);
        Assert.Equal("USD", result.Value.Currency);
        Assert.Equal(1000m, result.Value.Balance);

        mockRepo.Verify(r => r.CreateAccount(It.IsAny<Account>()), Times.Once);
    }

}

