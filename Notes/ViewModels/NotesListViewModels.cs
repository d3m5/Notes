using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Notes.Models;
using Notes.Services;
using Notes.Views;

#if WINDOWS
using System.Windows.Forms;
#endif

namespace Notes.ViewModels;

public sealed class NotesListViewModel : BaseViewModel
{
    private readonly INoteRepository _repository;
    private bool _isBusy;

    private List<Note> AllNotes = new();

    public ObservableCollection<Note> Notes { get; } = new();

    public ICommand LoadCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand SelectFolderCommand { get; }

    public NotesListViewModel(INoteRepository repository)
    {
        _repository = repository;

        LoadCommand = new Command(async () => await LoadAsync(), () => !IsBusy);
        AddCommand = new Command(async () => await OpenEditorAsync(Guid.Empty));
        EditCommand = new Command<Note>(async note => await OpenEditorAsync(note?.Id ?? Guid.Empty));
        DeleteCommand = new Command<Note>(async note => await DeleteAsync(note));
        SelectFolderCommand = new Command(async () => await SelectFolder());
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value))
                ((Command)LoadCommand).ChangeCanExecute();
        }
    }

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        try
        {
            AllNotes = (await _repository.GetAllAsync())
                       .OrderByDescending(n => n.UpdatedAt)
                       .ToList();

            Notes.Clear();

            foreach (var note in AllNotes)
                Notes.Add(note);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private string searchText = "";

    public string SearchText
    {
        get => searchText;
        set
        {
            searchText = value;
            FilterNotes();
            OnPropertyChanged();
        }
    }

    private void FilterNotes()
    {
        var filtered = AllNotes.Where(n =>
            n.Title.Contains(SearchText ?? "", StringComparison.OrdinalIgnoreCase) ||
            n.Content.Contains(SearchText ?? "", StringComparison.OrdinalIgnoreCase));

        Notes.Clear();

        foreach (var note in filtered)
            Notes.Add(note);
    }

    private static Task OpenEditorAsync(Guid id)
    {
        var route = id == Guid.Empty
            ? nameof(NoteEditorPage)
            : $"{nameof(NoteEditorPage)}?NoteId={id}";

        return Shell.Current.GoToAsync(route);
    }

    private async Task DeleteAsync(Note? note)
    {
        if (note is null)
            return;

        await _repository.DeleteAsync(note.Id);

        Notes.Remove(note);
        AllNotes.Remove(note);
    }

    private async Task SelectFolder()
    {
#if WINDOWS
        using var dialog = new FolderBrowserDialog();

        var result = dialog.ShowDialog();

        if (result == DialogResult.OK)
        {
            var repo = (FileNoteRepository)_repository;
            repo.SetFolder(dialog.SelectedPath);

            await LoadAsync();
        }
#endif
    }
}