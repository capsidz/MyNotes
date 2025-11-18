namespace MyNotesApi.Logging;

public static class PostgresLoggerExtensions
{
    /// <summary>
    /// Регистрирует PostgreSQL-логгер в стандартной системе логирования.
    /// </summary>
    public static ILoggingBuilder AddPostgresLogger(this ILoggingBuilder builder)
    {
        // регистрация провайдера как singleton
        builder.Services.AddSingleton<IPostgresLoggerProvider, PostgresLoggerProvider>();

        // регистрация фабрики
        builder.Services.AddSingleton<IPostgresLoggingFactory>(sp =>
            (IPostgresLoggingFactory)sp.GetRequiredService<IPostgresLoggerProvider>());

        // регистрация как стандартный ILoggerProvider,
        builder.Services.AddSingleton<ILoggerProvider>(sp =>
            (ILoggerProvider)sp.GetRequiredService<IPostgresLoggerProvider>());

        return builder;
    }
}