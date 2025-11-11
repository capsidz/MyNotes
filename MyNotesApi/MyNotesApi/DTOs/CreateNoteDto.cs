using System.ComponentModel.DataAnnotations;

namespace MyNotesApi.DTOs;
/// <summary>
/// DTO для создания новой заметки
/// </summary>
public class CreateNoteDto
{
    /// <summary>
    /// Заголовок заметки (обязательное поле, макс. 100 символов)
    /// </summary>
    /// <example>Список покупок</example>
    [Required(ErrorMessage = "Введите название!")]
    [StringLength(100, ErrorMessage = "Название не может быть длиннее 100 символов.")]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Содержание заметки (обязательное поле, мин. 10 символов)
    /// </summary>
    /// <example>Молоко, хлеб, яйца, фрукты для приготовления ужина</example>
    [Required(ErrorMessage = "Введите содержимое!")]
    [MinLength(5, ErrorMessage = "Содержимое не может быть короче 5 символов.")]
    public string Content { get; set; } = string.Empty;
}