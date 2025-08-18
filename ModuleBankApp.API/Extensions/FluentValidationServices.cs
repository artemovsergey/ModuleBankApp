using FluentValidation;

namespace ModuleBankApp.API.Extensions;

public static  class FluentValidationServices
{
    public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
    {
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        
        services.AddValidatorsFromAssemblyContaining<Program>();
        
        return services;
    }
}