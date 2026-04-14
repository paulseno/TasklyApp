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

        if (_isFromCompleted)
        {
            CompleteBtn.Text            = "Incomplete";
            CompleteBtn.BackgroundColor = Colors.Gray;
        }
    }

    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        string title  = TitleEntry.Text?.Trim()    ?? string.Empty;
        string detail = DetailsEditor.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Error", "Title cannot be empty.", "OK");
            return;
        }

        try
        {
            var response = await AppService.Api.UpdateItemAsync(_item.Id, title, detail);

            if (response.Status == 200)
            {
                _item.Title  = title;
                _item.Detail = detail;
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", response.Message, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not connect to server: {ex.Message}", "OK");
        }
    }

    private async void OnCompleteToggleClicked(object sender, EventArgs e)
    {
        // Mark inactive (done) if coming from active, or active if coming from completed
        string newStatus = _isFromCompleted ? "active" : "inactive";

        try
        {
            var response = await AppService.Api.ChangeStatusAsync(_item.Id, newStatus);

            if (response.Status == 200)
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
            else
            {
                await DisplayAlert("Error", response.Message, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not connect to server: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Delete", "Delete this task?", "Yes", "No");
        if (!confirm) return;

        try
        {
            var response = await AppService.Api.DeleteItemAsync(_item.Id);

            if (response.Status == 200)
            {
                if (_isFromCompleted)
                    AppService.CompletedItems.Remove(_item);
                else
                    AppService.TodoItems.Remove(_item);

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", response.Message, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not connect to server: {ex.Message}", "OK");
        }
    }
}