// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable NotAccessedPositionalProperty.Global
namespace ModuleBankApp.API.Models;

public record TokenResponse(
    string access_token,
    int expires_in,
    int refresh_expires_in,
    string refresh_token,
    string token_type,
    string id_token,
    string scope
);

