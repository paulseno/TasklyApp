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

    private void QuickCompleteItem(object sender, EventArgs e)
    {
        if (sender is Button btn && int.TryParse(btn.ClassId, out int id))
        {
            var item = AppService.TodoItems.FirstOrDefault(t => t.Id == id);
            if (item != null)
            {
                AppService.TodoItems.Remove(item);
                item.IsCompleted = true;
                AppService.CompletedItems.Add(item);
            }
        }
        UpdateEmptyState();
    }

    private void DeleteToDoItem(object sender, EventArgs e)
    {
        if (sender is Button btn && int.TryParse(btn.ClassId, out int id))
        {
            var item = AppService.TodoItems.FirstOrDefault(t => t.Id == id);
            if (item != null)
                AppService.TodoItems.Remove(item);
        }
        UpdateEmptyState();
    }

    private void UpdateEmptyState()
        => EmptyLabel.IsVisible = AppService.TodoItems.Count == 0;

    private async void OpenAddPage(object sender, EventArgs e)
        => await Shell.Current.Navigation.PushAsync(new AddPage());
}