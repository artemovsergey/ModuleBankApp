using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

namespace ModuleBankApp.API.Filters;

public class EnumTypesSchemaFilter : ISchemaFilter
{
    // ReSharper disable once NotAccessedField.Local
    private readonly string? _xmlPath;
    
    // ReSharper disable once UnusedMember.Global
    public EnumTypesSchemaFilter() { }
    
    public EnumTypesSchemaFilter(string xmlPath)
    {
        _xmlPath = xmlPath;
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Description = string.Join(", ",
                Enum.GetNames(context.Type)
                    .Select(name => $"{name} = {(int)Enum.Parse(context.Type, name)}"));
        }
    }
}