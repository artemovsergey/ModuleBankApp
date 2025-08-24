using System.Security.Claims;

namespace ModuleBankApp.API.Extensions;

public static class GetOwnerId
{
    public static Guid GetOwnerIdFromClaims(this ClaimsPrincipal user)
    {
        var ownerIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(ownerIdClaim, out var ownerId) ? ownerId : Guid.Empty;
    }
    
}

