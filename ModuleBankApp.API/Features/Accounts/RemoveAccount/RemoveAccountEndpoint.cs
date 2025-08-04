using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.RemoveAccount;

public static class RemoveAccountEndpoint
{
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapDelete("/account/{accountId:Guid}", async (
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
            
            var request = new RemoveAccountRequest(accountId, ownerId);
            var response = await mediator.Send(request);
            
            return response.IsSuccess 
                ? Results.Ok(response.Value) 
                : Results.BadRequest(response.Error);
        })
        .WithName("RemoveAccount")
        .WithSummary("Удаление счета")
        .WithDescription("Возвращает объект счета Account")
        .Produces<Account>(StatusCodes.Status200OK)
        .Produces<MbResult<Account>>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
        
        return app;
    }
}