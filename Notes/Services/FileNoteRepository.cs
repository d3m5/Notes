using System.Text.Json;
using Notes.Models;

namespace Notes.Services;

public class FileNoteRepository : INoteRepository
{
    private readonly string _filePath;
    //сохраняем файл в папку
    public FileNoteRepository()
    {
        _filePath = Path.Combine(FileSystem.AppDataDirectory, "notes.json");
    }

    private async Task<List<Note>> LoadAsync()
    {
        if (!File.Exists(_filePath))
            return new List<Note>();

        var json = await File.ReadAllTextAsync(_filePath);

        return JsonSerializer.Deserialize<List<Note>>(json) ?? new List<Note>();
    }

    private async Task SaveAsync(List<Note> notes)
    {
        var json = JsonSerializer.Serialize(notes, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task<List<Note>> GetAllAsync()
    {
        return await LoadAsync();
    }

    public async Task AddAsync(Note note)
    {
        var notes = await LoadAsync();
        notes.Add(note);
        await SaveAsync(notes);
    }

    public async Task UpdateAsync(Note note)
    {
        var notes = await LoadAsync();

        var existing = notes.FirstOrDefault(n => n.Id == note.Id);

        if (existing != null)
        {
            existing.Title = note.Title;
            existing.Content = note.Content;
            existing.UpdatedAt = DateTime.Now;
        }

        await SaveAsync(notes);
    }

    public async Task DeleteAsync(Guid id)
    {
        var notes = await LoadAsync();

        var note = notes.FirstOrDefault(n => n.Id == id);

        if (note != null)
            notes.Remove(note);

        await SaveAsync(notes);
    }

    public async Task<Note?> GetByIdAsync(Guid id)
    {
        var notes = await LoadAsync();
        return notes.FirstOrDefault(n => n.Id == id);
    }
}