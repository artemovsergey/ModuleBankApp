using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Extensions;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.CheckAccount;

// ReSharper disable once UnusedType.Global
public static class CheckAccountEndpoint
{
    // ReSharper disable once UnusedMember.Global
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapGet("/account/{accountId:Guid}", HandleEndpoint)
            .WithTags("Аккаунты")
            .WithName("GetAccount")
            .WithSummary("Поиск счета")
            .WithDescription("Возвращает объект счета Account")
            .Produces<Account>()
            .Produces<MbResult<Account>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> HandleEndpoint(
        Guid accountId,
        IMediator mediator,
        ClaimsPrincipal user
    )
    {
        var ownerId = user.GetOwnerIdFromClaims();
        if (ownerId == Guid.Empty) return Results.Unauthorized();

        var request = new CheckAccountRequest(accountId, ownerId);
        var response = await mediator.Send(request);

        return response.IsSuccess
            ? Results.Ok(response.Value)
            : Results.NotFound(response.Error);
    }
}

// +