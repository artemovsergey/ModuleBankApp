using System.Reflection;
using Microsoft.OpenApi.Models;
using ModuleBankApp.API.Filters;

namespace ModuleBankApp.API.Extensions;

public static class SwaggerServices
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        
        services.AddOpenApi();
        
        services.AddSwaggerGen(c =>
        {
            
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
    
            c.UseAllOfToExtendReferenceSchemas();
            c.SchemaFilter<EnumTypesSchemaFilter>(xmlPath);
            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Банковские счета",
                    Version = "v1",
                    Description = "Микросервис для управления банковскими счетами",
                    Contact = new OpenApiContact
                    {
                        Url = new Uri("https://github.com/artemovsergey/ModuleBankApp"),
                        Email = "artik3314@gmail.com"
                    }
                }
            );

            c.EnableAnnotations();
            c.SchemaFilter<ErrorResponseSchemaFilter>();
            c.OperationFilter<ErrorResponseOperationFilter>();
            
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description = "Authorization using jwt token. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                }
            );

            c.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        []
                    }
                }
            );
            
        });

        return services;
    }
}

// +