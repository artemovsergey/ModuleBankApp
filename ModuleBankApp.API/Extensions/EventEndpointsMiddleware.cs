using Microsoft.AspNetCore.Mvc;
using ModuleBankApp.API.Domen.Events;

namespace ModuleBankApp.API.Extensions;

// ReSharper disable once UnusedType.Global
public static class EventEndpointMiddleware
{
    // ReSharper disable once UnusedMember.Global
    public static WebApplication UseEventEndpointMiddleware(this WebApplication app)
    {
        
        app.MapPost("account_opened", ([FromBody] AccountOpened e) => Results.Ok())
            .WithTags("Events")
            .WithName("AccountOpened")
            .WithSummary("Событие создания счета")
            .WithDescription("Возвращает объект события AccountOpened");

        app.MapPost("client_blocked", ([FromBody] ClientBlocked e) => Results.Ok())
            .WithTags("Events")
            .WithName("ClientBlocked")
            .WithSummary("Событие блокирования клиента и всех его счетов")
            .WithDescription("Возвращает объект события ClientBlocked");

        app.MapPost("client_unblocked", ([FromBody] ClientUnblocked e) => Results.Ok())
            .WithTags("Events")
            .WithName("ClientUnblocked")
            .WithSummary("Событие разблокирования клиента и всех его счетов")
            .WithDescription("Возвращает объект события ClientUnblocked");

        app.MapPost("interest_accured", ([FromBody] InterestAccrued e) => Results.Ok())
            .WithTags("Events")
            .WithName("InterestAccrued")
            .WithSummary("Событие начисления процентов")
            .WithDescription("Возвращает объект события InterestAccrued");

        app.MapPost("money_credited", ([FromBody] MoneyCredited e) => Results.Ok())
            .WithTags("Events")
            .WithName("MoneyCredited")
            .WithSummary("Событие пополнения счета")
            .WithDescription("Возвращает объект события MoneyCredited");

        app.MapPost("money_debited", ([FromBody] MoneyDebited e) => Results.Ok())
            .WithTags("Events")
            .WithName("MoneyDebited")
            .WithSummary("Событие снятие средств со счета")
            .WithDescription("Возвращает объект события MoneyDebited");

        app.MapPost("transfer_сompleted", ([FromBody] TransferCompleted e) => Results.Ok())
            .WithTags("Events")
            .WithName("TransferCompleted")
            .WithSummary("Событие транзакции между счетами")
            .WithDescription("Возвращает объект события TransferCompleted");
        
        return app;
    }
}

