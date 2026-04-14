namespace ToDo_App;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        todoLV.ItemsSource = AppService.TodoItems;
        AppService.TodoItems.CollectionChanged += (_, _) => UpdateEmptyState();
        UpdateEmptyState();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateEmptyState();
    }

    private async void todoLV_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is ToDoClass selected)
            await Shell.Current.Navigation.PushAsync(new EditPage(selected, isFromCompleted: false));
    }

    private async void QuickCompleteItem(object sender, EventArgs e)
    {
        if (sender is Button btn && int.TryParse(btn.ClassId, out int id))
        {
            var item = AppService.TodoItems.FirstOrDefault(t => t.Id == id);
            if (item == null) return;

            try
            {
                var response = await AppService.Api.ChangeStatusAsync(item.Id, "inactive");
                if (response.Status == 200)
                {
                    AppService.TodoItems.Remove(item);
                    item.IsCompleted = true;
                    AppService.CompletedItems.Add(item);
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
        UpdateEmptyState();
    }

    private async void DeleteToDoItem(object sender, EventArgs e)
    {
        if (sender is Button btn && int.TryParse(btn.ClassId, out int id))
        {
            var item = AppService.TodoItems.FirstOrDefault(t => t.Id == id);
            if (item == null) return;

            try
            {
                var response = await AppService.Api.DeleteItemAsync(item.Id);
                if (response.Status == 200)
                {
                    AppService.TodoItems.Remove(item);
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
        UpdateEmptyState();
    }

    private void UpdateEmptyState()
        => EmptyLabel.IsVisible = AppService.TodoItems.Count == 0;

    private async void OpenAddPage(object sender, EventArgs e)
        => await Shell.Current.Navigation.PushAsync(new AddPage());
}