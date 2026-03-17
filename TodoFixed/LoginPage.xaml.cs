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

        bool hasAccount = !string.IsNullOrEmpty(AppService.Email);
        bool ok = !hasAccount || (email == AppService.Email && password == AppService.Password);

        if (!ok)
        {
            await DisplayAlert("Error", "Incorrect email or password.", "OK");
            return;
        }

        await Shell.Current.GoToAsync("//MainTabs");
    }

    private async void OnRegisterLabelTapped(object sender, TappedEventArgs e)
        => await Navigation.PushAsync(new RegisterPage());
}