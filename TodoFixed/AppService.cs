using System.Collections.ObjectModel;
using ToDo_App.Services;

namespace ToDo_App;

public static class AppService
{
    // Logged-in user info (populated after Sign In)
    public static int    UserId    { get; set; } = 0;
    public static string FirstName { get; set; } = string.Empty;
    public static string LastName  { get; set; } = string.Empty;
    public static string Email     { get; set; } = string.Empty;

    // Shared API service instance
    public static ApiService Api { get; } = new ApiService();

    // Local UI collections (synced from API)
    public static ObservableCollection<ToDoClass> TodoItems      { get; } = new();
    public static ObservableCollection<ToDoClass> CompletedItems { get; } = new();
}