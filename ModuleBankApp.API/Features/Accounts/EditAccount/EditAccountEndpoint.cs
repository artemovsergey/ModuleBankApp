using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.EditAccount;

public static class EditAccountEndpoint
{
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapPut("/account", async (
                EditAccountDto editAccountDto,
                IMediator mediator,
                ClaimsPrincipal user
            ) =>
            {
                var ownerIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(ownerIdClaim) || !Guid.TryParse(ownerIdClaim, out var ownerId))
                {
                    return Results.Unauthorized();
                }

                var request = new EditAccountRequest(editAccountDto, editAccountDto.Id, ownerId);
                var response = await mediator.Send(request);

                return response.IsSuccess
                    ? Results.Ok(response.Value)
                    : Results.BadRequest(response);
            })
            .WithName("UpdateAccount")
            .WithSummary("Обновление счета")
            .WithDescription("Возвращает объект обновленного счета Account")
            .Produces<Account>(StatusCodes.Status200OK)
            .Produces<MbResult<Account>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return app;
    }
}