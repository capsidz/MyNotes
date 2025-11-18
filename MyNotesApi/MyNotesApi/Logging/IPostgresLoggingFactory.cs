namespace MyNotesApi.Logging;

/// <summary>
/// Фабрика для создания PostgreSQL-логгеров.
/// </summary>
public interface IPostgresLoggingFactory
{
    IPostgresLogger CreateLogger(string categoryName);
}