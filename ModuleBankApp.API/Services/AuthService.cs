namespace ModuleBankApp.API.Services;

public interface IAuthService
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once UnusedParameter.Global
    bool CheckOwnerId(Guid ownerId);
}

public class AuthService : IAuthService
{
    public bool CheckOwnerId(Guid ownerId)
    {
        return true;
    }
}