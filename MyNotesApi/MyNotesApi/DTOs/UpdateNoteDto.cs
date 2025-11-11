using System.ComponentModel.DataAnnotations;

namespace MyNotesApi.DTOs;

/// <summary>
/// DTO для обновления существующей заметки
/// </summary>
public class UpdateNoteDto
{
    /// <summary>
    /// Новый заголовок заметки (необязательное поле)
    /// </summary>
    /// <example>Обновленный список покупок</example>
    [StringLength(100, ErrorMessage = "Название не может быть длиннее 100 символов.")]
    public string? Title { get; set; }
    
    /// <summary>
    /// Новое содержание заметки (необязательное поле)
    /// </summary>
    /// <example>Молоко, хлеб, яйца, сыр, йогурт, фрукты и овощи</example>
    [MinLength(5, ErrorMessage = "Содержимое не может быть короче 5 символов.")]
    public string? Content { get; set; } 
}