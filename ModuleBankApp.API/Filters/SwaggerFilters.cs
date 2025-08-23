using System.Reflection;
using System.Xml.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using ModuleBankApp.API.Generic;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ModuleBankApp.API.Filters;

public class EnumDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
        if (!File.Exists(xmlPath)) return;
        var xmlDoc = XDocument.Load(xmlPath);

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsEnum))
        {
            if (!context.SchemaRepository.Schemas.TryGetValue(type.Name, out var schema))
                continue;

            // Подставляем summary enum из XML
            var fullName = type.FullName!.Replace("+", ".");
            var member = xmlDoc.Descendants("member")
                .FirstOrDefault(m => m.Attribute("name")?.Value == $"T:{fullName}");
            schema.Description = member?.Element("summary")?.Value.Trim();

            // Переключаем тип на string и ставим enum имена
            schema.Type = "string";
            schema.Format = null;
            schema.Enum = type.GetEnumNames()
                .Select(n => (IOpenApiAny)new OpenApiString(n))
                .ToList<IOpenApiAny>();
        }
    }
}

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

// ReSharper disable once ClassNeverInstantiated.Global
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








