using System.Security.Claims;
using MediatR;
using ModuleBankApp.API.Generic;

namespace ModuleBankApp.API.Features.Accounts.GetAllAccounts;

public static class GetAllAccountsEndpoint
{
    public static WebApplication MapEndpoint(this WebApplication app)
    {
        app.MapGet("/accounts", async (
                IMediator mediator,
                ClaimsPrincipal user,
                ILoggerFactory logger
            ) =>
            {
                logger.CreateLogger("Банковские счета").LogInformation("Запрос на получение все счетов");
                var ownerIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(ownerIdClaim) || !Guid.TryParse(ownerIdClaim, out var ownerId))
                {
                    return Results.Unauthorized();
                    //ownerId = Guid.Parse("55555555-5555-5555-5555-555555555555");
                }
                
                var request = new GetAllAccountsRequest(ownerId);
                var response = await mediator.Send(request);

                return response.IsSuccess
                    ? Results.Ok(response.Value)
                    : Results.BadRequest(response.Error);
            })
            .WithName("GetAllAccounts")
            .WithSummary("Получение списка всех счетов")
            .WithDescription("Возвращает список счетов Account")
            .Produces<List<Account>>(StatusCodes.Status200OK)
            .Produces<MbResult<List<Account>>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

        return app;
    }
}