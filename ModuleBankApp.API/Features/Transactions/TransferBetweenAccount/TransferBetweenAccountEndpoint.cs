using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.TransferBetweenAccount;

public static class TransferBetweenAccountEndpoint
{
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapPost("/transaction/transfer", async (
                TransactionTransferDto transactionDto,
                IMediator mediator,
                ClaimsPrincipal user
                ) =>
        {
            
            var ownerIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                
            if (string.IsNullOrEmpty(ownerIdClaim) || !Guid.TryParse(ownerIdClaim, out var ownerId))
            {
                return Results.Unauthorized();
            }
            
            var request = new TransferBetweenAccountRequest(transactionDto, ownerId);
            var response = await mediator.Send(request);
            
            return response.IsSuccess 
                ? Results.Created($"/transaction/{response.Value.Id}", response.Value) 
                : Results.BadRequest(response.Error);
        })
        .WithTags("Транзакции по счетам")
        .WithName("CreateTransactionBetweenAccounts")
        .WithSummary("Создание трансфера между счетами")
        .WithDescription("Возвращает объект транзакции Transaction")
        .Produces<Transaction>(StatusCodes.Status201Created)
        .Produces<MbResult<Transaction>>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
        
        return app;
    }
}