using Notes.Models;

namespace Notes.Services;

public interface INoteRepository
{
    Task<List<Note>> GetAllAsync();

    Task<Note?> GetByIdAsync(Guid id);

    Task AddAsync(Note note);

    Task UpdateAsync(Note note);

    Task DeleteAsync(Guid id);
}