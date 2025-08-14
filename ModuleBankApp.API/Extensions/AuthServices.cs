using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ModuleBankApp.API.Extensions;

public static class AuthServices
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = $"{config["KeyCloakHost"]}/realms/ModulBankApp";
                options.Audience = "backend-api";
                options.RequireHttpsMetadata = false;
                options.MetadataAddress = $"{config["KeyCloakHost"]}/realms/ModulBankApp/.well-known/openid-configuration";
                options.TokenValidationParameters = new TokenValidationParameters
                {
          
                    ValidateAudience = true,
                    ValidAudiences = ["backend-api"],
            
                    ValidateLifetime = true,
            
                    ValidateIssuer = true,
                    ValidIssuers =
                    [
                        $"{config["KeyCloakHost"]}/realms/ModulBankApp"
                    ],
                    ValidateIssuerSigningKey = true
                };
            });
        
        return services;
    }
}

// +