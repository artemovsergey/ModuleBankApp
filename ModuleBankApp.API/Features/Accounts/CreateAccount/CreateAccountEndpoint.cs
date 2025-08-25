using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Extensions;
using ModuleBankApp.API.Generic;
using ModuleBankApp.API.Mappers;

namespace ModuleBankApp.API.Features.Accounts.CreateAccount;

public static class CreateAccountEndpoint
{
    // ReSharper disable once UnusedMember.Global
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapPost("/account", HandleEndpoint)
            .WithTags("Аккаунты")
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
        if (ownerId == Guid.Empty) return Results.Unauthorized();

        var request = new CreateAccountRequest(createAccountDto, ownerId);
        var response = await mediator.Send(request);

        return response.IsSuccess
            ? Results.CreatedAtRoute("GetAccount", new { accountId = response.Value.Id }, response.Value.ToDto())
            // ? Results.Created($"/account/{response.Value.Id}", response.Value)
            : Results.BadRequest(response);
    }
}

