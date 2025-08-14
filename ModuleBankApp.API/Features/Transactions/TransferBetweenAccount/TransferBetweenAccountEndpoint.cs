using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Extensions;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.TransferBetweenAccount;

// ReSharper disable once UnusedType.Global
public static class TransferBetweenAccountEndpoint
{
    
    // ReSharper disable once UnusedMember.Global
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        var policyName = app.Environment.IsEnvironment("Testing") ? "Allow" : "";
        app.MapPost("/transaction/transfer", HandleTransferBetweenAccounts)
            .WithTags("Транзакции по счетам")
            .WithName("CreateTransactionBetweenAccounts")
            .WithSummary("Создание трансфера между счетами")
            .WithDescription("Возвращает объект транзакции Transaction")
            .Produces<Transaction>(StatusCodes.Status201Created)
            .Produces<MbResult<Transaction>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(policyName);

        return app;
    }

    private static async Task<IResult> HandleTransferBetweenAccounts(
        TransactionDto transactionDto,
        IMediator mediator,
        ClaimsPrincipal user,
        IHostEnvironment env)
    {
        
        Guid ownerId;
        if (!env.IsEnvironment("Testing"))
        {
             ownerId = user.GetOwnerIdFromClaims();
             if (ownerId == Guid.Empty) return Results.Unauthorized();
        }
        else
        {
             ownerId = Guid.Parse("11111111-1111-1111-1111-111111111111"); 
        }
        
        var request = new TransferBetweenAccountRequest(transactionDto, ownerId);
        var response = await mediator.Send(request);

        if (response.Error == "Conflict")
            return Results.Conflict(
                "Операция не может быть выполнена из-за конфликта данных. Пожалуйста, повторите попытку позже.");

        return response.IsSuccess
            ? Results.Created("", response.Value)
            : Results.BadRequest(response.Error);
    }
}

// +