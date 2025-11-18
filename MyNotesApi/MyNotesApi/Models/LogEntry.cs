namespace MyNotesApi.Models;

/// <summary>
/// Запись лога, которая будет храниться в PostgreSQL
/// </summary>
public class LogEntry
{
    public int Id { get; set; }

    /// <summary>Время логирования (UTC)</summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>Уровень лога (Information, Error и т.п.)</summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>Категория (имя класса-логгера: NotesController)</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>Сообщение лога</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Текст исключения (если есть)</summary>
    public string? Exception { get; set; }
}