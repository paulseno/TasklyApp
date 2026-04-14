namespace MD_ToDo_List;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register all routes
        Routing.RegisterRoute("signin", typeof(Pages.SignInPage));
        Routing.RegisterRoute("signup", typeof(Pages.SignUpPage));
        Routing.RegisterRoute("mainpage", typeof(MainPage));
        Routing.RegisterRoute("profile", typeof(Pages.ProfilePage));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Navigate to Sign In on app start
        await Current.GoToAsync("signin");
    }
}