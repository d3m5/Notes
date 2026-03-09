using Microsoft.Extensions.Logging;
using Notes.Services;
using Notes.ViewModels;
using Notes.Views;


namespace Notes;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>();

        builder.Services.AddSingleton<INoteRepository, FileNoteRepository>();
        builder.Services.AddSingleton<NotesListViewModel>();
        builder.Services.AddTransient<NoteEditorViewModel>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<NotesPage>();
        builder.Services.AddTransient<NoteEditorPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}