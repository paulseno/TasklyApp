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

    public int Id
    {
        get => _id;
        set { _id = value; OnPropertyChanged(); }
    }
    public string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(); }
    }
    public string Detail
    {
        get => _detail;
        set { _detail = value; OnPropertyChanged(); }
    }
    public bool IsCompleted
    {
        get => _isCompleted;
        set { _isCompleted = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}