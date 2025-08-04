using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Transactions.IssueStatement;

public static class IssueStatementEndpoint
{
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapGet("/account/{accountId:Guid}/transactions", async (
                Guid accountId,
                IMediator mediator,
                ClaimsPrincipal user
            ) =>
            {
                var ownerIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(ownerIdClaim) || !Guid.TryParse(ownerIdClaim, out var ownerId))
                {
                    return Results.Unauthorized();
                }

                var request = new IssueStatementRequest(accountId, ownerId);
                var response = await mediator.Send(request);

                return response.IsSuccess
                    ? Results.Ok(response.Value)
                    : Results.BadRequest(response.Error);
            })
            .WithTags("Транзакции по счетам")
            .WithName("TransactionsByAccountId")
            .WithSummary("Выписка всех транзакций по счету")
            .WithDescription("Возвращает список транзакций List<Transaction>")
            .Produces<List<Transaction>>(StatusCodes.Status200OK)
            .Produces<MbResult<List<Transaction>>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return app;
    }
}