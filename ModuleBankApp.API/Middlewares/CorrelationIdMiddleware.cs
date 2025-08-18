namespace ModuleBankApp.API.Middlewares;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationHeader = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ILogger<CorrelationIdMiddleware> logger)
    {
        // 1. Читаем из заголовка или генерируем новый
        var correlationId = context.Request.Headers.TryGetValue(CorrelationHeader, out var value)
            ? value.ToString()
            : Guid.NewGuid().ToString();

        // 2. Кладём в HttpContext
        context.Items[CorrelationHeader] = correlationId;

        // 3. Добавляем в лог scope (будет автоматически попадать во все логи)
        using (logger.BeginScope(new Dictionary<string, object>
               {
                   ["CorrelationId"] = correlationId
               }))
        {
            // 4. Пробрасываем дальше
            await _next(context);
        }
    }
}
