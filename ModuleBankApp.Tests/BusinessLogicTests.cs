// using FluentAssertions;
// using ModuleBankApp.API.Data;
// using ModuleBankApp.API.Data.Interfaces;
// using ModuleBankApp.API.Features.Accounts;
// using ModuleBankApp.API.Features.Transactions;
// using ModuleBankApp.API.Features.Transactions.TransferBetweenAccount;
//
// using Xunit;
// using Moq;
// using Microsoft.Extensions.Logging;
// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
//
// namespace ModuleBankApp.Tests;
//
// public class TransferBetweenAccountHandlerTests
// {
//     private readonly ModuleBankAppContext _dbContext;
//     private readonly Mock<ITransactionRepository> _transactionRepo = new();
//     private readonly Mock<IAccountRepository> _accountRepo = new();
//     private readonly Mock<ILogger<TransferBetweenAccountHandler>> _logger = new();
//
//     public TransferBetweenAccountHandlerTests()
//     {
//         var options = new DbContextOptionsBuilder<ModuleBankAppContext>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString())
//             .Options;
//
//         _dbContext = new ModuleBankAppContext(options);
//     }
//
//     [Fact]
//     public async Task Handle_Should_TransferFunds_WhenBalanceIsSufficient()
//     {
//         // Arrange
//         var sender = new Account { Id = Guid.NewGuid(), Balance = 1000, Currency = "RUB" };
//         var receiver = new Account { Id = Guid.NewGuid(), Balance = 500, Currency = "RUB" };
//
//         _accountRepo.Setup(r => r.GetAccounById(sender.Id)).ReturnsAsync(sender);
//         _accountRepo.Setup(r => r.GetAccounById(receiver.Id)).ReturnsAsync(receiver);
//         _transactionRepo.Setup(r => r.RegisterTransaction(It.IsAny<Transaction>()))
//                         .ReturnsAsync((Transaction t) => t);
//
//         var handler = new TransferBetweenAccountHandler(
//             _transactionRepo.Object, _accountRepo.Object, _dbContext, _logger.Object);
//
//         var dto = new TransactionDto
//         {
//             AccountId = sender.Id,
//             CounterPartyAccountId = receiver.Id,
//             Amount = 100,
//             Currency = "RUB",
//             Type = TransactionType.Debit,
//             Description = "Test"
//         };
//
//         // Act
//         var result = await handler.Handle(new TransferBetweenAccountRequest(dto, Guid.NewGuid()), CancellationToken.None);
//
//         // Assert
//         result.IsSuccess.Should().BeTrue();
//         sender.Balance.Should().Be(900);
//         receiver.Balance.Should().Be(600);
//     }
//
//     // остальные тесты аналогично, используя _dbContext как реальный объект
// }
//
//
//
