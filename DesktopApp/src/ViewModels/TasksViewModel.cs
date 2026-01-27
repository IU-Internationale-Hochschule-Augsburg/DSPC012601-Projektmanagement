using System.Collections.ObjectModel;
using System.Windows;
using Projektmanagement_DesktopApp.Models;
using Projektmanagement_DesktopApp.Repositories;

namespace Projektmanagement_DesktopApp.ViewModels;

public class TasksViewModel : ViewModelBase
{
    private readonly ITaskRepository _taskRepository;
    private bool _isAddingNew;
    private TaskModel? _selectedTask;
    private int _newDuration;

    public TasksViewModel(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        Tasks = new ObservableCollection<TaskModel>();
        
        AddCommand = new RelayCommand(_ => StartAdding());
        SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
        CancelCommand = new RelayCommand(_ => CancelAdding());
        SelectCommand = new RelayCommand(t => SelectedTask = t as TaskModel);
        
        _ = LoadAsync();
    }

    public ObservableCollection<TaskModel> Tasks { get; }

    public bool IsAddingNew
    {
        get => _isAddingNew;
        set
        {
            if (SetProperty(ref _isAddingNew, value))
            {
                OnPropertyChanged(nameof(HeaderText));
                OnPropertyChanged(nameof(IsListVisible));
            }
        }
    }

    public string HeaderText => IsAddingNew ? "Neue Aufgabe" : "Aufgaben";
    public bool IsListVisible => !IsAddingNew;

    public TaskModel? SelectedTask
    {
        get => _selectedTask;
        set => SetProperty(ref _selectedTask, value);
    }

    public int NewDuration
    {
        get => _newDuration;
        set
        {
            if (SetProperty(ref _newDuration, value))
            {
                ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public RelayCommand AddCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand SelectCommand { get; }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        try
        {
            var data = await _taskRepository.GetAllAsync();
            Tasks.Clear();
            foreach (var item in data) Tasks.Add(item);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Laden: {ex.Message}");
        }
    }

    private void StartAdding()
    {
        NewDuration = 0;
        IsAddingNew = true;
    }

    private bool CanSave() => NewDuration > 0;

    private async System.Threading.Tasks.Task SaveAsync()
    {
        try
        {
            var model = new TaskModel { Duration = NewDuration };
            var saved = await _taskRepository.AddAsync(model);
            Tasks.Add(saved);
            IsAddingNew = false;
            SelectedTask = saved;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Speichern: {ex.Message}");
        }
    }

    private void CancelAdding() => IsAddingNew = false;
}
