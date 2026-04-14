namespace ToDo_App;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class ToDoClass : INotifyPropertyChanged
{
    public ToDoClass() { }

    private int    _id;
    private string _title       = string.Empty;
    private string _detail      = string.Empty;
    private bool   _isCompleted = false;

    // Maps to API's item_id
    public int Id
    {
        get => _id;
        set { _id = value; OnPropertyChanged(); }
    }

    // Maps to API's item_name
    public string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(); }
    }

    // Maps to API's item_description
    public string Detail
    {
        get => _detail;
        set { _detail = value; OnPropertyChanged(); }
    }

    // true = "inactive" (done), false = "active"
    public bool IsCompleted
    {
        get => _isCompleted;
        set { _isCompleted = value; OnPropertyChanged(); }
    }

    // Helper: convert API status string to IsCompleted bool
    public static ToDoClass FromDto(Services.TodoDto dto) => new ToDoClass
    {
        Id          = dto.ItemId,
        Title       = dto.ItemName,
        Detail      = dto.ItemDescription,
        IsCompleted = dto.Status == "inactive"
    };

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}