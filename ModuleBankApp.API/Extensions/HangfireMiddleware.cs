using Hangfire;
using MediatR;
using ModuleBankApp.API.Filters;
using ModuleBankApp.API.Handlers;
using ModuleBankApp.API.Services;

namespace ModuleBankApp.API.Extensions;

public static class HangfireMiddleware
{
    public static WebApplication UseHangfireMiddleware(this WebApplication app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = [new HangfireAuthorizationFilter()]
        });
        
        app.MapPost("/test-accrue-interest", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new AccrueInterestRequest());
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        });
        app.MapPost("/test-cron-job", () =>
        {
            RecurringJob.TriggerJob("accrue-interest-job");
            return Results.Ok("Cron job triggered");
        });
        
        RecurringJob.AddOrUpdate<InterestJobService>(
            "accrue-interest-job",
            job => job.AccrueInterest(),
            Cron.Daily);
        
        return app;
    }
}