using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNotesApi.Data;
using MyNotesApi.DTOs;
using MyNotesApi.Models;

namespace MyNotesApi.Controllers;


/// <summary>
/// Контроллер для работы с заметками
/// </summary>
[ApiController]
[Route("notes")]
public class NotesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<NotesController> _logger;

    public NotesController(AppDbContext context, ILogger<NotesController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Получить все активные заметки
    /// </summary>
    /// <returns>Список всех заметок</returns>
    /// <response code="200">Возвращает список заметок</response>
    /// <response code="500">Произошла внутренняя ошибка сервера</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
    {
        _logger.LogInformation("Получение заметки");

        try
        {
            var notes = await _context.Notes
                .Where(note => !note.IsDeleted)
                .OrderByDescending(note => note.CreatedAt)
                .ToListAsync();
            
            _logger.LogInformation("Получено {Count} заметок", notes.Count);
            return Ok(notes);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Произошла ошибка при получении заметок");
            return StatusCode(500, "Ошибка при получении заметок");;
        }
    }

    /// <summary>
    /// Получить заметку по ID
    /// </summary>
    /// <param name="id">ID заметки</param>
    /// <returns>Заметка</returns>
    /// <response code="200">Возвращает запрошенную заметку</response>
    /// <response code="404">Заметка не найдена</response>
    /// <response code="500">Произошла внутренняя ошибка сервера</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<Note>> GetNote(int id)
    {
        _logger.LogInformation("Получение заметки с ID: {NoteId}", id);

        try
        {
            var note = await _context.Notes
                .FirstOrDefaultAsync(note => note.Id == id && !note.IsDeleted);
            
            if (note == null)
            {
                _logger.LogWarning("Заметка с ID: {NoteId} не найдена", id);
                return NotFound(new { message = $"Заметка с ID: {id} не найдена" });
            }
            
            return Ok(note);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Произошла ошибка при получении заметки с ID: {NoteId}", id);
            return StatusCode(500, "Произошла ошибка при получении заметки.");
        }
    }

    /// <summary>
    /// Создать новую заметку
    /// </summary>
    /// <param name="createDto">DTO для создания заметки</param>
    /// <returns>Созданная заметка</returns>
    /// <response code="201">Заметка успешно создана</response>
    /// <response code="400">Невалидные данные</response>
    /// <response code="500">Произошла внутренняя ошибка сервера</response>
    [HttpPost]
    public async Task<ActionResult<Note>> CreateNote([FromBody] CreateNoteDto createDto)
    {
        _logger.LogInformation("Создание новой заметки с названием: {Title}", createDto.Title);
            
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Недопустимое ModelState для создания заметки: {Errors}", 
                string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return BadRequest(ModelState);
        }

        try
        {
            var note = new Note
            {
                Title = createDto.Title,
                Content = createDto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Создана заметка с ID: {NoteId}", note.Id);
                
            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при создании заметки");
            return StatusCode(500, "Ошибка при создании заметки");
        }
    }

    /// <summary>
    /// Обновить существующую заметку
    /// </summary>
    /// <param name="id">ID заметки для обновления</param>
    /// <param name="updateDto">DTO с обновляемыми полями</param>
    /// <returns>Обновленная заметка</returns>
    /// <response code="200">Заметка успешно обновлена</response>
    /// <response code="400">Невалидные данные</response>
    /// <response code="404">Заметка не найдена</response>
    /// <response code="500">Произошла внутренняя ошибка сервера</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteDto updateDto)
    {
        _logger.LogInformation("Изменение заметки с ID: {NoteId}", id);
            
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Недопустимое ModelState для изменения заметки: {Errors}", 
                string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return BadRequest(ModelState);
        }

        try
        {
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);
            if (note == null || note.IsDeleted)
            {
                _logger.LogWarning("Заметка с ID {NoteId} для изменения не найдена", id);
                return NotFound(new { message = $"Заметка с ID {id} не найдена" });
            }
            
            if (!string.IsNullOrEmpty(updateDto.Title))
                note.Title = updateDto.Title;
                
            if (!string.IsNullOrEmpty(updateDto.Content))
                note.Content = updateDto.Content;
                
            note.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Успешно изменена заметка с ID: {NoteId}", id);
            return Ok(note);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при изменении заметки с ID {NoteId}", id);
            return StatusCode(500, "Ошибка при изменении заметки");
        }
    }
    
    /// <summary>
    /// Удалить заметку
    /// </summary>
    /// <param name="id">ID заметки для удаления</param>
    /// <returns>Статус операции</returns>
    /// <response code="204">Заметка успешно удалена</response>
    /// <response code="404">Заметка не найдена</response>
    /// <response code="500">Произошла внутренняя ошибка сервера</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        _logger.LogInformation("Удаление заметки с ID: {NoteId}", id);
            
        try
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null || note.IsDeleted)
            {
                _logger.LogWarning("Заметка с ID {NoteId} для удаления не найдена", id);
                return NotFound(new { message = $"Заметка с ID {id} не найдена" });
            }
            
            note.IsDeleted = true;
            note.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Успещное удаление заметки с ID: {NoteId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка при удалении заметки с ID {NoteId}", id);
            return StatusCode(500, "Ошибка при удалении заметки");
        }
    }
}