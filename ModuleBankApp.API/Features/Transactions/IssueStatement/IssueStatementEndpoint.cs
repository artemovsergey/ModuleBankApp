using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Extensions;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.IssueStatement;

// ReSharper disable once UnusedType.Global
public static class IssueStatementEndpoint
{
    // ReSharper disable once UnusedMember.Global
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapGet("/account/{accountId:Guid}/transactions", IssueStatementHandler)
            .WithTags("Транзакции по счетам")
            .WithName("TransactionsByAccountId")
            .WithSummary("Выписка всех транзакций по счету")
            .WithDescription("Возвращает список транзакций List<Transaction>")
            .Produces<List<Transaction>>()
            .Produces<MbResult<List<Transaction>>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return app;
    }
    
    private static async Task<IResult> IssueStatementHandler(
        Guid accountId,
        IMediator mediator,
        ClaimsPrincipal user)
    {
        var ownerId = user.GetOwnerIdFromClaims();
        if (ownerId == Guid.Empty) return Results.Unauthorized();

        var request = new IssueStatementRequest(accountId, ownerId);
        var response = await mediator.Send(request);

        return response.IsSuccess
            ? Results.Ok(response.Value)
            : Results.BadRequest(response.Error);
    }
}

