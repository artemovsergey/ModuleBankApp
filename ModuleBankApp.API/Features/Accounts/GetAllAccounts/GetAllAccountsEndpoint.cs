using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Extensions;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.GetAllAccounts;

// ReSharper disable once UnusedType.Global
public static class GetAllAccountsEndpoint
{
    // ReSharper disable once UnusedMember.Global
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapGet("/accounts", HandleGetAllAccounts)
            .WithName("GetAllAccounts")
            .WithSummary("Получение списка всех счетов")
            .WithDescription("Возвращает список счетов Account")
            .Produces<List<Account>>()
            .Produces<MbResult<List<Account>>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> HandleGetAllAccounts(
        this IMediator mediator,
        ClaimsPrincipal user,
        ILoggerFactory logger)
    {
        logger.CreateLogger("Банковские счета").LogInformation("Запрос на получение все счетов");
                
        var ownerId = user.GetOwnerIdFromClaims();
        if (ownerId == Guid.Empty) return Results.Unauthorized();
                
        var request = new GetAllAccountsRequest(ownerId);
        var response = await mediator.Send(request);

        return response.IsSuccess
            ? Results.Ok(response.Value)
            : Results.BadRequest(response.Error);
    }
}

// +