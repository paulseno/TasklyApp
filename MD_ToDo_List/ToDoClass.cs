namespace MD_ToDo_List;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class ToDoClass : INotifyPropertyChanged
{
    public ToDoClass() { }

    private int _item_id;
    private string _item_name = string.Empty;
    private string _item_description = string.Empty;
    private string _status = "active";
    private int _user_id;
    private string _timemodified = string.Empty;

    public int item_id
    {
        get { return _item_id; }
        set { _item_id = value; OnPropertyChanged(nameof(item_id)); }
    }

    public string item_name
    {
        get { return _item_name; }
        set { _item_name = value; OnPropertyChanged(nameof(item_name)); }
    }

    public string item_description
    {
        get { return _item_description; }
        set { _item_description = value; OnPropertyChanged(nameof(item_description)); }
    }

    public string status
    {
        get { return _status; }
        set { _status = value; OnPropertyChanged(nameof(status)); }
    }

    public int user_id
    {
        get { return _user_id; }
        set { _user_id = value; OnPropertyChanged(nameof(user_id)); }
    }

    public string timemodified
    {
        get { return _timemodified; }
        set { _timemodified = value; OnPropertyChanged(nameof(timemodified)); }
    }

    // Properties for backward compatibility
    public int id
    {
        get { return _item_id; }
        set { _item_id = value; OnPropertyChanged(nameof(id)); }
    }

    public string title
    {
        get { return _item_name; }
        set { _item_name = value; OnPropertyChanged(nameof(title)); }
    }

    public string detail
    {
        get { return _item_description; }
        set { _item_description = value; OnPropertyChanged(nameof(detail)); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}