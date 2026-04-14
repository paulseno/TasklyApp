namespace ToDo_App;

public partial class AddPage : ContentPage
{
    public AddPage() { InitializeComponent(); }

    private async void AddToDoItem(object sender, EventArgs e)
    {
        string title  = TitleEntry.Text?.Trim()        ?? string.Empty;
        string detail = DetailsEditor.Text?.Trim()     ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Error", "Please enter a task title.", "OK");
            return;
        }

        try
        {
            var response = await AppService.Api.AddItemAsync(AppService.UserId, title, detail);

            if (response.Status == 200 && response.Data != null)
            {
                // Add to local list using data returned from server (gets real item_id)
                AppService.TodoItems.Add(ToDoClass.FromDto(response.Data));
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