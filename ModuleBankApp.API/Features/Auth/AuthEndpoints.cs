using ModuleBankApp.API.Models;

namespace ModuleBankApp.API.Features.Auth;

public static class AuthEndpoints
{
    public static WebApplication UseLoginEndpoint(this WebApplication app, IConfiguration config)
    {
        app.MapPost("/auth/login", async (LoginRequest request, IHttpClientFactory httpClientFactory) =>
            {
                var httpClient = httpClientFactory.CreateClient();

                var formData = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "client_id", "backend-api" },
                    { "username", request.Username },
                    { "password", request.Password }
                };

                var response = await httpClient.PostAsync(
                    $"{config["KeyCloakHost"]}/realms/ModulBankApp/protocol/openid-connect/token",
                    new FormUrlEncodedContent(formData)
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Results.Json(new
                    {
                        error = $"{errorContent}",
                        description = "Invalid username or password"
                    }, statusCode: StatusCodes.Status401Unauthorized);
                }

                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return Results.Ok(tokenResponse);
            }).WithName("Login")
            .WithSummary("Аутентификация")
            .WithDescription("Возвращает jwt токен с Guid пользователя")
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
        
        return app;
    }
}