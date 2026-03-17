namespace ToDo_App;
public partial class AddPage : ContentPage
{
    public AddPage() { InitializeComponent(); }

    private async void AddToDoItem(object sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Error", "Please enter a task title.", "OK");
            return;
        }
        AppService.TodoItems.Add(new ToDoClass
        {
            Id     = AppService.TodoItems.Count + AppService.CompletedItems.Count + 1,
            Title  = title,
            Detail = DetailsEditor.Text?.Trim() ?? string.Empty
        });
        await Navigation.PopAsync();
    }
}