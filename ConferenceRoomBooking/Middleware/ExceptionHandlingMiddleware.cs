using System.Net;
using System.Text.Json;
using ConferenceBooking.Application.Common.Exceptions;

namespace ConferenceBooking.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, title, errors) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                "Помилка валідації",
                (object?)validationEx.Errors),

            NotFoundException => (
                HttpStatusCode.NotFound,
                exception.Message,
                null),

            BusinessRuleException => (
                HttpStatusCode.Conflict,
                exception.Message,
                null),

            ArgumentException => (
                HttpStatusCode.BadRequest,
                exception.Message,
                null),

            _ => (
                HttpStatusCode.InternalServerError,
                "Сталася непередбачена помилка. Спробуйте пізніше.",
                null)
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Необроблена помилка");
        }
           
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = (int)statusCode,
            title,
            errors
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}