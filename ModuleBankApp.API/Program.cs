using System.Reflection;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Behaviors;
using ModuleBankApp.API.Data;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Data.Repositories;
using ModuleBankApp.API.Extensions;
using ModuleBankApp.API.Features.Auth;
using ModuleBankApp.API.Handlers;
using ModuleBankApp.API.Metrics;
using ModuleBankApp.API.Services;
using Hangfire;
using Hangfire.PostgreSql;
using ModuleBankApp.API.Filters;


var builder = WebApplication.CreateBuilder(args);

ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

var config = builder.Configuration;
builder.Services.AddSwaggerServices();
builder.Services.AddSerializeServices();
builder.Services.AddAuthServices(config);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
        .AddOpenBehavior(typeof(HandlePerformancemetricBehavior<,>))
);

builder.Services.AddTransient(typeof(IRequestPreProcessor<>), typeof(LoggingBehavior<>));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddSingleton<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddDbContext<ModuleBankAppContext>(o =>
    o.UseNpgsql(config.GetConnectionString("PostgreSQL")));

builder.Services.AddSingleton<HandlePerformancemetric>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", c =>
    {
        c.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


builder.Services.AddHangfire(x => x.UsePostgreSqlStorage(options =>
{
    options.UseNpgsqlConnection(config.GetConnectionString("HangfirePostgreSQL"));
}));

builder.Services.AddHangfireServer();


builder.Services.AddTransient<InterestJobService>();

builder.Services.AddSingleton<IEventBus>(sp =>
    RabbitMqEventBus.CreateAsync("guest", "guest", "/", "rabbitmq").GetAwaiter().GetResult());

var app = builder.Build();


if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ModuleBankAppContext>();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.Migrate();
    var sql = File.ReadAllText("Data/Procedures/accrue_interest.sql");
    await dbContext.Database.ExecuteSqlRawAsync(sql);
}

app.UseCors("AllowAll");

if (!builder.Environment.IsEnvironment("Testing"))
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.UseLoginEndpoint(config);
app.UseEndpointsRegister();

app.UseSwaggerMiddleware();


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


app.Run();

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once RedundantTypeDeclarationBody
public partial class Program
{
}

// +