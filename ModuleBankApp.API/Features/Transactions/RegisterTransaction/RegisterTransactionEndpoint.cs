using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Dtos;
using ModuleBankApp.API.Extensions;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.RegisterTransaction;

// ReSharper disable once UnusedType.Global
public static class RegisterTransactionEndpoint
{
    // ReSharper disable once UnusedMember.Global
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapPost("/transaction", RegisterTransactionHandler)
            .WithTags("Транзакции по счетам")
            .WithName("CreateTransaction")
            .WithSummary("Создание новой транзакции")
            .WithDescription("Возвращает объект транзакции Transaction")
            .Produces<Transaction>(StatusCodes.Status201Created)
            .Produces<MbResult<Transaction>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> RegisterTransactionHandler(
        TransactionDto transactionDto,
        IMediator mediator,
        ClaimsPrincipal user)
    {
        var ownerId = user.GetOwnerIdFromClaims();
        if (ownerId == Guid.Empty) return Results.Unauthorized();

        var request = new RegisterTransactionRequest(transactionDto, ownerId);
        var response = await mediator.Send(request);

        return response.IsSuccess
            ? Results.Created("", response.Value)
            : Results.BadRequest(response.Error);
    }
}