using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Extensions;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.RemoveAccount;

// ReSharper disable once UnusedType.Global
public static class RemoveAccountEndpoint
{
    // ReSharper disable once UnusedMember.Global
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapDelete("/account/{accountId:Guid}", HandleRemoveAccount)
            .WithName("RemoveAccount")
            .WithSummary("Удаление счета")
            .WithDescription("Возвращает объект счета Account")
            .Produces<Account>()
            .Produces<MbResult<Account>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> HandleRemoveAccount(
        this IMediator mediator,
        Guid accountId,
        ClaimsPrincipal user)
    {
        var ownerId = user.GetOwnerIdFromClaims();
        if (ownerId == Guid.Empty) return Results.Unauthorized();

        var request = new RemoveAccountRequest(accountId);
        var response = await mediator.Send(request);

        return response.IsSuccess
            ? Results.Ok(response.Value)
            : Results.BadRequest(response.Error);
    }
}

// +