namespace ToDo_App;

public partial class ProfilePage : ContentPage
{
    public ProfilePage() { InitializeComponent(); }

    public void NotifyAppearing() => RefreshData();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshData();
    }

    private void RefreshData()
    {
        UsernameLabel.Text = string.IsNullOrEmpty(AppService.Username) ? "Guest" : AppService.Username;
        EmailLabel.Text    = string.IsNullOrEmpty(AppService.Email)    ? "No email set" : AppService.Email;
        PendingCount.Text  = AppService.TodoItems.Count.ToString();
        DoneCount.Text     = AppService.CompletedItems.Count.ToString();
    }

    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Sign Out", "Are you sure you want to sign out?", "Yes", "No");
        if (!confirm) return;
        await Shell.Current.GoToAsync("//LoginPage");
    }
}