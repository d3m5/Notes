using System.Globalization;
using System.Windows.Input;
using Notes.Services;
using Notes.ViewModels;
using Notes.Models;

namespace Notes.ViewModels;

public sealed class NoteEditorViewModel : BaseViewModel, IQueryAttributable
{
    private readonly INoteRepository _repository;
    private Guid _noteId;
    private string _title = string.Empty;
    private string _content = string.Empty;
    private string _screenTitle = "Новая заметка";

    public NoteEditorViewModel(INoteRepository repository)
    {
        _repository = repository;
        SaveCommand = new Command(async () => await SaveAsync());
        CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    public string ScreenTitle
    {
        get => _screenTitle;
        private set => SetProperty(ref _screenTitle, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _noteId = Guid.Empty;
        Title = string.Empty;
        Content = string.Empty;
        ScreenTitle = "Новая заметка";

        if (!query.TryGetValue("NoteId", out var rawValue))
        {
            return;
        }

        var text = Convert.ToString(rawValue, CultureInfo.InvariantCulture);
        if (!Guid.TryParse(text, out var id))
        {
            return;
        }

        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
        {
            return;
        }

        _noteId = existing.Id;
        Title = existing.Title;
        Content = existing.Content;
        ScreenTitle = "Редактирование";
    }

    private async Task SaveAsync()
    {
        var cleanTitle = Title?.Trim() ?? string.Empty;
        var cleanContent = Content?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(cleanTitle) && string.IsNullOrWhiteSpace(cleanContent))
        {
            await Shell.Current.DisplayAlert("Пустая заметка", "Заполните заголовок или текст.", "OK");
            return;
        }

        if (_noteId == Guid.Empty)
        {
            await _repository.AddAsync(new Note
            {
                Id = Guid.NewGuid(),
                Title = cleanTitle,
                Content = cleanContent,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            await _repository.UpdateAsync(new Note
            {
                Id = _noteId,
                Title = cleanTitle,
                Content = cleanContent,
                UpdatedAt = DateTime.UtcNow
            });
        }

        await Shell.Current.GoToAsync("..");
    }
}