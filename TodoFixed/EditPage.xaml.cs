namespace ToDo_App;

public partial class EditPage : ContentPage
{
    private readonly ToDoClass _item;
    private readonly bool      _isFromCompleted;

    public EditPage(ToDoClass item, bool isFromCompleted)
    {
        InitializeComponent();
        _item            = item;
        _isFromCompleted = isFromCompleted;

        TitleEntry.Text    = item.Title;
        DetailsEditor.Text = item.Detail;

        // Swap button label/color depending on which list we came from
        if (_isFromCompleted)
        {
            CompleteBtn.Text            = "Incomplete";
            CompleteBtn.BackgroundColor = Colors.Gray;
        }
    }

    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Error", "Title cannot be empty.", "OK");
            return;
        }
        _item.Title  = title;
        _item.Detail = DetailsEditor.Text?.Trim() ?? string.Empty;
        await Navigation.PopAsync();
    }

    private async void OnCompleteToggleClicked(object sender, EventArgs e)
    {
        if (!_isFromCompleted)
        {
            _item.Title  = TitleEntry.Text?.Trim() ?? _item.Title;
            _item.Detail = DetailsEditor.Text?.Trim() ?? _item.Detail;
            AppService.TodoItems.Remove(_item);
            _item.IsCompleted = true;
            AppService.CompletedItems.Add(_item);
        }
        else
        {
            AppService.CompletedItems.Remove(_item);
            _item.IsCompleted = false;
            AppService.TodoItems.Add(_item);
        }
        await Navigation.PopAsync();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Delete", "Delete this task?", "Yes", "No");
        if (!confirm) return;

        if (_isFromCompleted)
            AppService.CompletedItems.Remove(_item);
        else
            AppService.TodoItems.Remove(_item);

        await Navigation.PopAsync();
    }
}