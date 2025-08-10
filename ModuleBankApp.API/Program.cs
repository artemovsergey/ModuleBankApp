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
using ModuleBankApp.API.Metrics;
using ModuleBankApp.API.Services;

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
builder.Services.AddSingleton<IAuthService, AuthService>();

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

var app = builder.Build();

if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ModuleBankAppContext>();
    dbContext.Database.Migrate();
}
else
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ModuleBankAppContext>();
    dbContext.Database.Migrate();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.UseLoginEndpoint(config);
app.UseEndpointsRegister();

app.UseSwaggerMiddleware();
app.Run();