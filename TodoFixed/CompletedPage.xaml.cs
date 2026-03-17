namespace ToDo_App;

public partial class CompletedPage : ContentPage
{
    public CompletedPage()
    {
        InitializeComponent();
        completedLV.ItemsSource = AppService.CompletedItems;
        AppService.CompletedItems.CollectionChanged += (_, _) => UpdateEmptyState();
        UpdateEmptyState();
    }

    public void NotifyAppearing() => UpdateEmptyState();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateEmptyState();
    }

    private void DeleteCompletedItem(object sender, EventArgs e)
    {
        if (sender is Button btn && int.TryParse(btn.ClassId, out int id))
        {
            var item = AppService.CompletedItems.FirstOrDefault(t => t.Id == id);
            if (item != null)
                AppService.CompletedItems.Remove(item);
        }
        UpdateEmptyState();
    }

    private async void completedLV_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is ToDoClass selected)
            await Shell.Current.Navigation.PushAsync(new EditPage(selected, isFromCompleted: true));
    }

    private void UpdateEmptyState()
        => EmptyLabel.IsVisible = AppService.CompletedItems.Count == 0;
}