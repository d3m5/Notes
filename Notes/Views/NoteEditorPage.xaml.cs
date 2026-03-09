using Notes.ViewModels;

namespace Notes.Views;

[QueryProperty(nameof(NoteId), "NoteId")]
public partial class NoteEditorPage : ContentPage
{
    public NoteEditorPage(NoteEditorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public string NoteId
    {
        set
        {
            if (BindingContext is IQueryAttributable queryTarget)
            {
                queryTarget.ApplyQueryAttributes(new Dictionary<string, object>
                {
                    ["NoteId"] = Uri.UnescapeDataString(value ?? string.Empty)
                });
            }
        }
    }
}
