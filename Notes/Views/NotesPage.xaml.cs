using Notes.ViewModels;

namespace Notes.Views;

public partial class NotesPage : ContentPage
{
    private readonly NotesListViewModel _viewModel;

    public NotesPage(NotesListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}
