using System.Collections.ObjectModel;

namespace ToDo_App;

public static class AppService
{
    public static string Username  { get; set; } = string.Empty;
    public static string Email     { get; set; } = string.Empty;
    public static string Password  { get; set; } = string.Empty;

    public static ObservableCollection<ToDoClass> TodoItems      { get; } = new();
    public static ObservableCollection<ToDoClass> CompletedItems { get; } = new();
}