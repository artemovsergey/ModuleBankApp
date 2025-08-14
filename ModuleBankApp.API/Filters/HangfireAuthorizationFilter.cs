using Hangfire.Dashboard;

namespace ModuleBankApp.API.Filters;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}

// +