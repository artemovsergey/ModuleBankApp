using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.CreateAccount;

public static class CreateAccountEndpoint
{
    private static AuthorizationPolicy _authorizationPolicy =>
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim("feature", "createaccount")
            .Build();
    
    private static AuthorizationPolicy _authorizationPolicyAction =>
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim("scope", "write")
            .Build();
    
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapPost("/account", async (
                AccountDto accountDto,
                IMediator mediator,
                ClaimsPrincipal user
                ) =>
        {
            
            var ownerIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                
            if (string.IsNullOrEmpty(ownerIdClaim) || !Guid.TryParse(ownerIdClaim, out var ownerId))
            {
                return Results.Unauthorized();
            }
            
            var request = new CreateAccountRequest(accountDto, ownerId);
            var response = await mediator.Send(request);
            
            return response.IsSuccess 
                ? Results.Created($"/account/{response.Value.Id}", response.Value) 
                : Results.BadRequest(response);
        })
        .WithName("CreateAccount")
        .WithSummary("Создание нового счета")
        .WithDescription("Возвращает объект счета Account")
        .Produces<Account>(StatusCodes.Status201Created)
        .Produces<MbResult<Account>>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
        // .RequireAuthorization(_authorizationPolicy);
        
        return app;
    }
}