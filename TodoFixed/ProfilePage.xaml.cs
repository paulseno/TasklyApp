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
        string fullName = $"{AppService.FirstName} {AppService.LastName}".Trim();
        UsernameLabel.Text = string.IsNullOrEmpty(fullName) ? "Guest" : fullName;
        EmailLabel.Text    = string.IsNullOrEmpty(AppService.Email) ? "No email set" : AppService.Email;
        PendingCount.Text  = AppService.TodoItems.Count.ToString();
        DoneCount.Text     = AppService.CompletedItems.Count.ToString();
    }

    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Sign Out", "Are you sure you want to sign out?", "Yes", "No");
        if (!confirm) return;

        // Clear user session on sign out
        AppService.UserId    = 0;
        AppService.FirstName = string.Empty;
        AppService.LastName  = string.Empty;
        AppService.Email     = string.Empty;
        AppService.TodoItems.Clear();
        AppService.CompletedItems.Clear();

        await Shell.Current.GoToAsync("//LoginPage");
    }
}