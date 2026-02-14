using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using Projektmanagement_DesktopApp.Models;
using Projektmanagement_DesktopApp.Repositories;
using Projektmanagement_DesktopApp.Services;

namespace Projektmanagement_DesktopApp.ViewModels;

public class TasksViewModel : ViewModelBase
{
    private readonly ITaskRepository _taskRepository;
    private readonly TaskService _taskService;
    private bool _isAddingNew;
    private TaskModel? _selectedTask;
    private int _newDuration;
    private string _newDescription = string.Empty;

    public TasksViewModel(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _taskService = new TaskService(taskRepository);
        Tasks = new ObservableCollection<TaskModel>();
        
        AddCommand = new RelayCommand(_ => StartAdding());
        SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
        CancelCommand = new RelayCommand(_ => CancelAdding());
        SelectCommand = new RelayCommand(t => SelectedTask = t as TaskModel);
        RecalculateTimelineCommand = new RelayCommand(
            async _ => await RecalculateTimelineAsync(),
            _ => SelectedTask != null && SelectedTask.ProjectId > 0);
        
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
        set
        {
            if (SetProperty(ref _selectedTask, value))
            {
                ((RelayCommand)RecalculateTimelineCommand).RaiseCanExecuteChanged();
            }
        }
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

    public string NewDescription
    {
        get => _newDescription;
        set => SetProperty(ref _newDescription, value);
    }

    public RelayCommand AddCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand SelectCommand { get; }
    public RelayCommand RecalculateTimelineCommand { get; }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        try
        {
            var data = await _taskRepository.GetAllAsync();
            Tasks.Clear();
            foreach (var item in data) Tasks.Add(item);
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show($"Fehler beim Laden: {ex.Message}");
        }
    }

    private void StartAdding()
    {
        NewDuration = 0;
        NewDescription = string.Empty;
        IsAddingNew = true;
    }

    private bool CanSave() => NewDuration > 0;

    private async System.Threading.Tasks.Task SaveAsync()
    {
        try
        {
            var model = new TaskModel
            {
                Duration = NewDuration,
                Description = NewDescription.Trim()
            };
            var saved = await _taskRepository.AddAsync(model);
            Tasks.Add(saved);
            IsAddingNew = false;
            SelectedTask = saved;
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show($"Fehler beim Speichern: {ex.Message}");
        }
    }

    private void CancelAdding() => IsAddingNew = false;

    /// <summary>
    /// Interaktive Entscheidungsfindung: Zeigt vorgeschlagene Terminänderungen und
    /// lässt den Nutzer entscheiden, ob der Projektplan aktualisiert wird.
    /// </summary>
    private async System.Threading.Tasks.Task RecalculateTimelineAsync()
    {
        if (SelectedTask == null || SelectedTask.ProjectId <= 0)
            return;

        try
        {
            int projectId = SelectedTask.ProjectId;

            // Vorschau berechnen
            var proposals = await _taskService.PreviewRecalculationAsync(projectId);
            var changes = proposals.Where(p => p.HasChanged).ToList();

            if (changes.Count == 0)
            {
                MessageBox.Show(
                    "Alle Termine sind bereits aktuell. Keine Änderungen erforderlich.",
                    "Timeline aktuell",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            // Zusammenfassung der Änderungen erstellen
            var sb = new StringBuilder();
            sb.AppendLine($"Es werden {changes.Count} Aufgabe(n) aktualisiert:\n");

            foreach (var change in changes)
            {
                sb.AppendLine($"• {change.TaskDisplayName}");
                sb.AppendLine($"    Start: {change.OldStartDate:dd.MM.yyyy HH:mm} → {change.NewStartDate:dd.MM.yyyy HH:mm}");
                sb.AppendLine($"    Ende:  {change.OldEndDate:dd.MM.yyyy HH:mm} → {change.NewEndDate:dd.MM.yyyy HH:mm}");
                sb.AppendLine();
            }

            sb.AppendLine("Möchten Sie diese Änderungen übernehmen?");

            // Nutzer entscheidet
            var result = MessageBox.Show(
                sb.ToString(),
                "Timeline aktualisieren – Vorschau",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Kalkulation mit Persistierung durchführen
                await _taskService.RecalculateProjectTimelineAsync(projectId, persistChanges: true);

                // Liste aktualisieren
                await LoadAsync();

                MessageBox.Show(
                    "Die Timeline wurde erfolgreich aktualisiert.",
                    "Erfolg",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(
                $"Fehler bei der Timeline-Berechnung:\n{ex.Message}",
                "Fehler",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
