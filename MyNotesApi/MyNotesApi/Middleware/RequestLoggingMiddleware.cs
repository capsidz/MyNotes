namespace MyNotesApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;

        // логирование входящего запроса
        _logger.LogInformation("Входящий запрос: {Method} {Path} в {Time}",
            context.Request.Method,
            context.Request.Path,
            startTime.ToString("HH:mm:ss.fff"));

        try
        {
            await _next(context);

            var endTime = DateTime.UtcNow;
            var duration = (endTime - startTime).TotalMilliseconds;

            _logger.LogInformation("Ответ: {StatusCode} в течении {Duration} мс в {Time}",
                context.Response.StatusCode,
                duration,
                endTime.ToString("HH:mm:ss.fff"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка обработки запроса: {Method} {Path}",
                context.Request.Method, context.Request.Path);
            throw;
        }
    }
}