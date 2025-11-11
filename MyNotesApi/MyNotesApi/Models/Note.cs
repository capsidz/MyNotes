using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyNotesApi.Models;

/// <summary>
/// Модель заметки
/// </summary>
public class Note
{
    /// <summary>
    /// Уникальный идентификатор заметки
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }
    
    /// <summary>
    /// Заголовок заметки
    /// </summary>
    /// <example>Моя первая заметка</example>
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Содержание заметки
    /// </summary>
    /// <example>Это содержание моей первой заметки</example>
    [Required]
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Дата и время создания заметки
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }  
    
    /// <summary>
    /// Флаг мягкого удаления
    /// </summary>
    /// <example>false</example>
    [JsonIgnore] // Скрываем от API responses
    public bool IsDeleted { get; set; } = false;
}