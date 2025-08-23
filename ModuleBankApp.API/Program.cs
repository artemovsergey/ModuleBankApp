using System.Reflection;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Behaviors;
using ModuleBankApp.API.Data.Interfaces;
using ModuleBankApp.API.Domen.Events;
using ModuleBankApp.API.Extensions;
using ModuleBankApp.API.Features.Auth;
using ModuleBankApp.API.Infrastructure.Data;
using ModuleBankApp.API.Infrastructure.Data.Interfaces;
using ModuleBankApp.API.Infrastructure.Data.Repositories;
using ModuleBankApp.API.Infrastructure.Messaging;
using ModuleBankApp.API.Services;
using ModuleBankApp.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Host.AddSerilogConfiguration(config);
builder.Services.AddSwaggerServices();
builder.Services.AddSerializeServices();
builder.Services.AddAuthServices(config);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddTransient(typeof(IRequestPreProcessor<>), typeof(LoggingBehavior<>));
builder.Services.AddFluentValidationServices();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddSingleton<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddDbContext<ModuleBankAppContext>(o =>
    o.UseNpgsql(config.GetConnectionString("PostgreSQL")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AllowAll", policy =>
    {
        policy.RequireAssertion(_ => true); // всегда разрешено
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", c =>
    {
        c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.AddHangfireServices(config);
builder.Services.AddEventBusServices(config);

builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthCheckServices(config);

var app = builder.Build();

var connection = app.Services.GetRequiredService<IEventBusConnection>();
await EventBusSetup.SetupQueuesAsync(connection, "account.events");

// app.UseMiddleware<CorrelationIdMiddleware>();
if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ModuleBankAppContext>();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.Migrate();
    var sql = File.ReadAllText("Infrastructure/Data/Procedures/accrue_interest.sql");
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
app.UseHealthMiddleware();
app.UseHangfireMiddleware();
app.UseEventEndpointMiddleware();

app.Run();

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once RedundantTypeDeclarationBody
public partial class Program { }