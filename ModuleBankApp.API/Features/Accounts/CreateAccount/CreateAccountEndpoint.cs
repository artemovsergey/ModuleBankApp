using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Extensions;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.CreateAccount;

public static class CreateAccountFunction
{
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapPost("/account", HandleEndpoint)
            .WithTags("Account")
            .WithName("CreateAccount")
            .WithSummary("Создание нового счета")
            .WithDescription("Возвращает объект счета Account")
            .Produces<Account>(StatusCodes.Status201Created)
            .Produces<MbResult<Account>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> HandleEndpoint(
        CreateAccountDto createAccountDto,
        IMediator mediator,
        ClaimsPrincipal user)
    {
        var ownerId = user.GetOwnerIdFromClaims();
        if (ownerId is null) return Results.Unauthorized();

        var request = new CreateAccountRequest(createAccountDto, ownerId);
        var response = await mediator.Send(request);

        return response.IsSuccess
            ? Results.Created($"/account/{response.Value.Id}", response.Value)
            : Results.BadRequest(response);
    }
}