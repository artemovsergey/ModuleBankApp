using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Extensions;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.EditAccount;

// ReSharper disable once UnusedType.Global
public static class EditAccountEndpoint
{
    // ReSharper disable once UnusedMember.Global
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapPut("/account", HandleEditAccount)
            .WithName("UpdateAccount")
            .WithSummary("Обновление счета")
            .WithDescription("Возвращает объект обновленного счета Account")
            .Produces<Account>()
            .Produces<MbResult<Account>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> HandleEditAccount(
        this IMediator mediator,
        EditAccountDto editAccountDto,
        ClaimsPrincipal user)
    {
        var ownerId = user.GetOwnerIdFromClaims();
        if (ownerId == Guid.Empty) return Results.Unauthorized();

        var request = new EditAccountRequest(editAccountDto, editAccountDto.Id, ownerId);
        var response = await mediator.Send(request);

        return response.IsSuccess
            ? Results.Ok(response.Value)
            : Results.BadRequest(response);
    }
}

// +