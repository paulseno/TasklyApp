namespace ToDo_App;

public partial class RegisterPage : ContentPage
{
    public RegisterPage() { InitializeComponent(); }

    private async void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        string firstName = UserRegisterInput.Text?.Trim()  ?? string.Empty;
        string email     = EmailRegisterInput.Text?.Trim() ?? string.Empty;
        string password  = PassRegisterInput.Text          ?? string.Empty;
        string confirm   = ConfirmPassInput.Text           ?? string.Empty;

        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please fill in all fields.", "OK");
            return;
        }
        if (password != confirm)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        try
        {
            // API requires first_name and last_name — use firstName for both if no last name field
            var response = await AppService.Api.SignUpAsync(
                firstName,
                lastName:        string.Empty,  // add a LastName field to your UI if needed
                email:           email,
                password:        password,
                confirmPassword: confirm
            );

            if (response.Status == 200)
            {
                await DisplayAlert("Success", "Account created! You can now sign in.", "OK");
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

    private async void OnLoginLabelTapped(object sender, TappedEventArgs e)
        => await Navigation.PopAsync();
}