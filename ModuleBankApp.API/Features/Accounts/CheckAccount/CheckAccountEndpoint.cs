using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.CheckAccount;

public static class CheckAccountEndpoint
{
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapGet("/account/{accountId:Guid}", async (
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

                var request = new CheckAccountRequest(accountId);
                var response = await mediator.Send(request);

                return response.IsSuccess
                    ? Results.Ok(response.Value)
                    : Results.NotFound(response.Error);
            })
            .WithName("GetAccount")
            .WithSummary("Поиск счета")
            .WithDescription("Возвращает объект счета Account")
            .Produces<Account>(StatusCodes.Status200OK)
            .Produces<MbResult<Account>>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
        
        return app;
    }
}