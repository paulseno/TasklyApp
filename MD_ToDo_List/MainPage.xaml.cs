namespace MD_ToDo_List;
using System.Collections.ObjectModel;
using MD_ToDo_List.Services;

public partial class MainPage : ContentPage
{
    private ObservableCollection<ToDoClass> toDoList = new ObservableCollection<ToDoClass>();
    private ToDoClass? selectedToDo = null;
    private IApiService _apiService;
    private IAuthService _authService;
    private bool isLoading = false;

    public MainPage()
    {
        InitializeComponent();
        todoLV.ItemsSource = toDoList;
        toDoList.CollectionChanged += (s, e) => UpdateUI();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        
        // Initialize services when handler is ready
        if (Handler != null && _apiService == null)
        {
            var serviceProvider = this.Handler.MauiContext?.Services;
            if (serviceProvider != null)
            {
                _apiService = serviceProvider.GetService<IApiService>();
                _authService = serviceProvider.GetService<IAuthService>();
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Try to restore user session from storage
        if (_authService?.IsAuthenticated != true)
        {
            // Try to load from SecureStorage
            var userId = await SecureStorage.Default.GetAsync("user_id");
            var userEmail = await SecureStorage.Default.GetAsync("user_email");
            var userFname = await SecureStorage.Default.GetAsync("user_fname");
            var userLname = await SecureStorage.Default.GetAsync("user_lname");

            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int id) && id > 0)
            {
                // Restore session
                var userData = new SignInResponse
                {
                    id = id,
                    email = userEmail,
                    fname = userFname,
                    lname = userLname
                };
                _authService?.SetUserData(userData);
            }
            else
            {
                await Shell.Current.GoToAsync("signin");
                return;
            }
        }

        // Load tasks from server first, fall back to local storage
        await LoadToDoItems();
        
        // If no tasks loaded, try loading from local storage
        if (toDoList.Count == 0)
        {
            await LoadTasksLocally();
        }
    }

    private async Task LoadToDoItems()
    {
        if (isLoading || _authService == null || _apiService == null) 
            return;

        // Validate CurrentUserId
        if (_authService.CurrentUserId <= 0)
        {
            await DisplayAlert("Error", "User ID is invalid. Please sign in again.", "OK");
            await Shell.Current.GoToAsync("signin");
            return;
        }
            
        isLoading = true;

        try
        {
            var response = await _apiService.GetToDoItemsAsync("active", _authService.CurrentUserId);

            if (response.Status == 200 && response.Data != null)
            {
                toDoList.Clear();
                foreach (var item in response.Data)
                {
                    toDoList.Add(new ToDoClass
                    {
                        item_id = item.item_id,
                        item_name = item.item_name ?? string.Empty,
                        item_description = item.item_description ?? string.Empty,
                        status = item.status ?? "active",
                        user_id = item.user_id,
                        timemodified = item.timemodified ?? string.Empty
                    });
                }
                // Save to local storage
                await SaveTasksLocally(toDoList);
            }
            else if (response.Message?.Contains("not authenticated") == true || response.Status == 401)
            {
                await DisplayAlert("Session Expired", "Please sign in again.", "OK");
                _authService?.ClearUserData();
                SecureStorage.Default.Remove("user_id");
                SecureStorage.Default.Remove("user_email");
                await Shell.Current.GoToAsync("signin");
            }
            else
            {
                await DisplayAlert("Error", response.Message ?? "Failed to load tasks", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error loading tasks: {ex.Message}", "OK");
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task SaveTasksLocally(ObservableCollection<ToDoClass> tasks)
    {
        try
        {
            // Use Preferences to save tasks as JSON
            var json = System.Text.Json.JsonSerializer.Serialize(tasks);
            Preferences.Default.Set($"tasks_{_authService?.CurrentUserId}", json);
        }
        catch { }
    }

    private async Task LoadTasksLocally()
    {
        try
        {
            var json = Preferences.Default.Get($"tasks_{_authService?.CurrentUserId}", null as string);
            if (!string.IsNullOrEmpty(json))
            {
                var tasks = System.Text.Json.JsonSerializer.Deserialize<List<ToDoClass>>(json);
                if (tasks != null)
                {
                    toDoList.Clear();
                    foreach (var task in tasks)
                    {
                        toDoList.Add(task);
                    }
                }
            }
        }
        catch { }
    }

    private async void AddToDoItem(object sender, EventArgs e)
    {
        string title = titleEntry.Text?.Trim() ?? string.Empty;
        string detail = detailsEditor.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Validation", "Please enter a title for the task.", "OK");
            return;
        }

        addBtn.IsEnabled = false;
        addBtn.Text = "Adding...";

        try
        {
            var request = new AddToDoRequest
            {
                item_name = title,
                item_description = detail,
                user_id = _authService?.CurrentUserId ?? 0
            };

            var response = await _apiService.AddToDoItemAsync(request);

            if (response.Status == 200 && response.Data != null)
            {
                toDoList.Add(new ToDoClass
                {
                    item_id = response.Data.item_id,
                    item_name = response.Data.item_name ?? string.Empty,
                    item_description = response.Data.item_description ?? string.Empty,
                    status = response.Data.status ?? "active",
                    user_id = response.Data.user_id,
                    timemodified = response.Data.timemodified ?? string.Empty
                });
                ClearInputs();
                await DisplayAlert("Success", "Task added successfully!", "OK");
            }
            else
            {
                await DisplayAlert("Error", response.Message ?? "Failed to add task", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error adding task: {ex.Message}", "OK");
        }
        finally
        {
            addBtn.IsEnabled = true;
            addBtn.Text = "Add";
        }
    }

    private void todoLV_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is ToDoClass tapped)
        {
            selectedToDo = tapped;
            titleEntry.Text = tapped.item_name;
            detailsEditor.Text = tapped.item_description;

            addBtn.IsVisible = false;
            editBtn.IsVisible = true;
            cancelBtn.IsVisible = true;
        }
    }

    private void TodoLV_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        todoLV.SelectedItem = null;
    }

    private async void EditToDoItem(object sender, EventArgs e)
    {
        if (selectedToDo == null) return;

        string title = titleEntry.Text?.Trim() ?? string.Empty;
        string detail = detailsEditor.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Validation", "Title cannot be empty.", "OK");
            return;
        }

        editBtn.IsEnabled = false;
        editBtn.Text = "Saving...";

        try
        {
            var request = new UpdateToDoRequest
            {
                item_id = selectedToDo.item_id,
                item_name = title,
                item_description = detail
            };

            var response = await _apiService.UpdateToDoItemAsync(request);

            if (response.Status == 200)
            {
                selectedToDo.item_name = title;
                selectedToDo.item_description = detail;

                // Refresh the list
                todoLV.ItemsSource = null;
                todoLV.ItemsSource = toDoList;

                await DisplayAlert("Success", "Task updated successfully!", "OK");
                CancelEdit(sender, e);
            }
            else
            {
                await DisplayAlert("Error", response.Message ?? "Failed to update task", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error updating task: {ex.Message}", "OK");
        }
        finally
        {
            editBtn.IsEnabled = true;
            editBtn.Text = "Save";
        }
    }

    private void CancelEdit(object sender, EventArgs e)
    {
        selectedToDo = null;
        ClearInputs();
        addBtn.IsVisible = true;
        editBtn.IsVisible = false;
        cancelBtn.IsVisible = false;
    }

    private async void DeleteToDoItem(object sender, EventArgs e)
    {
        if (sender is Button btn && int.TryParse(btn.ClassId, out int itemId))
        {
            bool confirm = await DisplayAlert("Confirm", "Are you sure you want to delete this task?", "Yes", "No");
            
            if (!confirm) return;

            try
            {
                var response = await _apiService.DeleteToDoItemAsync(itemId);

                if (response.Status == 200)
                {
                    var item = toDoList.FirstOrDefault(t => t.item_id == itemId);
                    if (item != null)
                    {
                        toDoList.Remove(item);
                        if (selectedToDo?.item_id == itemId)
                            CancelEdit(sender, e);
                    }
                    await DisplayAlert("Success", "Task deleted successfully!", "OK");
                }
                else
                {
                    await DisplayAlert("Error", response.Message ?? "Failed to delete task", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error deleting task: {ex.Message}", "OK");
            }
        }
    }

    private async void OnProfileClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("profile");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (confirm)
        {
            _authService?.ClearUserData();
            await Shell.Current.GoToAsync("signin");
        }
    }

    private void ClearInputs()
    {
        titleEntry.Text = string.Empty;
        detailsEditor.Text = string.Empty;
    }

    private void UpdateUI()
    {
        taskCountLabel.Text = toDoList.Count > 0 ? $"{toDoList.Count} task(s)" : "No tasks yet";
    }
}