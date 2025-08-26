using ModuleBankApp.API.Infrastructure.Data;
using ModuleBankApp.API.Infrastructure.Messaging.Models;

namespace ModuleBankApp.API.Infrastructure.Messaging.Consumers;

public sealed class ConsumerBase
{
    public static async Task MoveToErrorLetterAsync(
        IServiceScopeFactory scopeFactory,
        string handler,
        string payload,
        string error,
        ILogger log,
        CancellationToken ct)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ModuleBankAppContext>();

            var deadLetter = new InboxErrorLetter
            {
                Id = Guid.NewGuid(),
                ReceivedAtUtc = DateTimeOffset.UtcNow,
                Handler = handler,
                Payload = payload,
                Error = error
            };

            db.InboxErrorLetters.Add(deadLetter);
            await db.SaveChangesAsync(ct);

            log.LogWarning("Сообщение отправлено в error-letter. Причина: {Error}", error);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Ошибка при сохранении error-letter");
        }
    }

}

