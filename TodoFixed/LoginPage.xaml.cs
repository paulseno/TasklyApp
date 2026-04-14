namespace ToDo_App;

public partial class LoginPage : ContentPage
{
    public LoginPage() { InitializeComponent(); }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        string email    = EmailInput.Text?.Trim() ?? string.Empty;
        string password = PassInput.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please enter your Email and Password.", "OK");
            return;
        }

        // Show loading indicator if you have one, e.g.: LoadingIndicator.IsVisible = true;

        try
        {
            var response = await AppService.Api.SignInAsync(email, password);

            if (response.Status == 200 && response.Data != null)
            {
                // Save logged-in user info
                AppService.UserId    = response.Data.Id;
                AppService.FirstName = response.Data.FirstName;
                AppService.LastName  = response.Data.LastName;
                AppService.Email     = response.Data.Email;

                // Load tasks from API before navigating
                await LoadTasksAsync();

                await Shell.Current.GoToAsync("//MainTabs");
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

        // Hide loading indicator if you have one
    }

    private async Task LoadTasksAsync()
    {
        AppService.TodoItems.Clear();
        AppService.CompletedItems.Clear();

        // Load active tasks
        var activeResponse = await AppService.Api.GetItemsAsync(AppService.UserId, "active");
        if (activeResponse.Status == 200)
        {
            foreach (var kv in activeResponse.Data)
                AppService.TodoItems.Add(ToDoClass.FromDto(kv.Value));
        }

        // Load completed (inactive) tasks
        var doneResponse = await AppService.Api.GetItemsAsync(AppService.UserId, "inactive");
        if (doneResponse.Status == 200)
        {
            foreach (var kv in doneResponse.Data)
                AppService.CompletedItems.Add(ToDoClass.FromDto(kv.Value));
        }
    }

    private async void OnRegisterLabelTapped(object sender, TappedEventArgs e)
        => await Navigation.PushAsync(new RegisterPage());
}