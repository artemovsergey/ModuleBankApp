using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using ModuleBankApp.API.Generic;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ModuleBankApp.API.Filters;

public class ErrorResponseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsGenericType && context.Type.GetGenericTypeDefinition() == typeof(MbResult<>))
        {
            schema.Example = new OpenApiObject
            {
                ["isSuccess"] = new OpenApiBoolean(false),
                ["value"] = new OpenApiNull(),
                ["error"] = new OpenApiString("Validation failed: currency is not supported.")
            };
        }
    }
}


public class ErrorResponseOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var statusCodes = new Dictionary<string, (string Message, string Detail)>
        {
            ["400"] = ("Invalid request", "Параметры запроса невалидны"),
            ["401"] = ("Unauthorized", "Пользователь не аутентифицирован"),
            ["404"] = ("Resource not found", "Запрашиваемый ресурс не найден"),
            ["412"] = ("Precondition failed", "Несогласованное состояние объекта"),
            ["500"] = ("Internal server error", "Внутренняя ошибка сервера")
        };

        foreach (var (code, (message, detail)) in statusCodes)
        {
            if (operation.Responses.TryGetValue(code, out var response) &&
                response.Content.TryGetValue("application/json", out var mediaType))
            {
                mediaType.Example = new OpenApiObject
                {
                    ["isSuccess"] = new OpenApiBoolean(false),
                    ["value"] = null,
                    ["error"] = new OpenApiString($"{message}: {detail}")
                };
            }
        }
    }
}
