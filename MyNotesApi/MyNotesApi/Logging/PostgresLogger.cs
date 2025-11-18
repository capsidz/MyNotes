using MyNotesApi.Data;
using MyNotesApi.Models;

namespace MyNotesApi.Logging;


/// <summary>
/// Реализация логгера, который пишет в PostgreSQL через EF Core.
/// </summary>
public class PostgresLogger : IPostgresLogger
{
    private readonly string _categoryName;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly bool _isEfMigration;

    public PostgresLogger(string categoryName, IServiceScopeFactory scopeFactory)
    {
        _categoryName = categoryName;
        _scopeFactory = scopeFactory;
        
        // проверка: не выполняется ли мы миграцию EF
        _isEfMigration = Environment.GetEnvironmentVariable("EF_CORE_COMMAND") == "true" ||
                         AppDomain.CurrentDomain.GetAssemblies()
                             .Any(a => a.FullName?.Contains("Microsoft.EntityFrameworkCore.Tools") == true);
    }

    public IDisposable? BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId,
        TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (_isEfMigration || !IsEnabled(logLevel))
            return;
        
        var message = formatter(state, exception);

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var entry = new LogEntry
            {
                Timestamp = DateTime.UtcNow,
                Level = logLevel.ToString(),
                Category = _categoryName,
                Message =  message.Length > 1000 ? message.Substring(0, 1000) : message,
                Exception = exception?.ToString()?[..500]
            };

            context.Logs.Add(entry);
            context.SaveChanges();
        }
        catch
        {
            // запись лога в БД не удалась
        }
    }
}