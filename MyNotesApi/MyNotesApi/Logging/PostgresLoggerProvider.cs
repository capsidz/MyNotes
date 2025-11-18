namespace MyNotesApi.Logging;

/// <summary>
/// Провайдер PostgreSQL логгеров.
/// </summary>
public class PostgresLoggerProvider : IPostgresLoggerProvider, IPostgresLoggingFactory
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PostgresLoggerProvider(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public ILogger CreateLogger(string categoryName)
        => new PostgresLogger(categoryName, _scopeFactory);

    IPostgresLogger IPostgresLoggingFactory.CreateLogger(string categoryName)
        => new PostgresLogger(categoryName, _scopeFactory);

    public void Dispose()
    {
        // ничего 
    }
}