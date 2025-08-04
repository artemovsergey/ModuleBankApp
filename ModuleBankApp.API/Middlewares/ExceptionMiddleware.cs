using System.Data.SqlTypes;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ModuleBankApp.API.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlerMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json"; // Стандартный ContentType для ProblemDetails
        
        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path,
            Detail = _env.IsDevelopment() ? exception.StackTrace : null,
            Extensions = { ["traceId"] = context.TraceIdentifier }
        };

        switch (exception)
        {
            // case Npgsql.PostgresException ex:
            //     problemDetails.Title = "Database error";
            //     problemDetails.Status = (int)HttpStatusCode.InternalServerError;
            //     problemDetails.Detail = ex.Message;
            //     problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
            //     break;
                
            case BadHttpRequestException ex:
                problemDetails.Title = "Invalid request";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Detail = ex.Message;
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
                break;
                
            // case NotFoundException ex:
            //     problemDetails.Title = "Not found";
            //     problemDetails.Status = (int)HttpStatusCode.NotFound;
            //     problemDetails.Detail = ex.Message;
            //     problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";
            //     break;
                
            case SqlTypeException ex:
                problemDetails.Title = "Data type error";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Detail = ex.Message;
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1";
                break;
                
            default:
                problemDetails.Title = "An unexpected error occurred";
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
                break;
        }

        context.Response.StatusCode = problemDetails.Status.Value;
        
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _env.IsDevelopment()
        };
        
        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }
}
