using System.Security.Claims;

namespace ModuleBankApp.API.Services;

// ReSharper disable once UnusedType.Global
public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    // ReSharper disable once UnusedMember.Global
    public string? UserId => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
}

// +