using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ModuleBankApp.API.Domen.Events;
using ModuleBankApp.API.Filters;

namespace ModuleBankApp.API.Extensions;

public static class SwaggerServices
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        
        //services.AddOpenApi();
        
        services.AddSwaggerGen(c =>
        {
      
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            
            c.IncludeXmlComments(xmlPath);
            c.UseAllOfToExtendReferenceSchemas();
            
            c.SwaggerDoc(
                "api",
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
            
            c.SwaggerDoc(
                "events",
                new OpenApiInfo
                {
                    Title = "События предметной области",
                    Version = "v1",
                    Description = "Контракты событий (EventBus) для интеграции"
                }
            );
            
            c.DocumentFilter<EnumDocumentFilter>();
            
            // фильтруем, какие endpoints попадают в какой документ
            c.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (docName == "api")
                {
                    // в API идут все, кроме событий
                    return !apiDesc.ActionDescriptor.EndpointMetadata
                        .OfType<TagsAttribute>()
                        .Any(t => t.Tags.Contains("Events"));
                }

                if (docName == "events")
                {
                    // в Events идут только помеченные тегом "Events"
                    if (apiDesc.ActionDescriptor.EndpointMetadata
                        .OfType<TagsAttribute>()
                        .Any(t => t.Tags.Contains("Events")))
                    {
                        return true;
                    }

                    // или если вход/выход реализует IEvent
                    var returnType = apiDesc.ActionDescriptor.EndpointMetadata
                        .OfType<ProducesResponseTypeAttribute>()
                        .Select(a => a.Type)
                        .FirstOrDefault();

                    if (returnType != null && typeof(IEvent).IsAssignableFrom(returnType))
                        return true;
                }

                return false;
            });
            
            
            c.EnableAnnotations();
            
            c.SchemaFilter<ErrorResponseSchemaFilter>();
            c.SchemaFilter<EnumTypesSchemaFilter>(xmlPath);
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

