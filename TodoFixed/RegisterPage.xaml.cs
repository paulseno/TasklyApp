namespace ToDo_App;
public partial class RegisterPage : ContentPage
{
    public RegisterPage() { InitializeComponent(); }

    private async void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        string username = UserRegisterInput.Text?.Trim()  ?? string.Empty;
        string email    = EmailRegisterInput.Text?.Trim() ?? string.Empty;
        string password = PassRegisterInput.Text          ?? string.Empty;
        string confirm  = ConfirmPassInput.Text           ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please fill in all fields.", "OK");
            return;
        }
        if (password != confirm)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        AppService.Username = username;
        AppService.Email    = email;
        AppService.Password = password;

        await DisplayAlert("Success", "Account created! You can now sign in.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnLoginLabelTapped(object sender, TappedEventArgs e)
        => await Navigation.PopAsync();
}