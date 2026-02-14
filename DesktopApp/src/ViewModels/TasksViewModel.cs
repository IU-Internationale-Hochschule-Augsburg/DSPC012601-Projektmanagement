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
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkerRepository _workerRepository;
    private readonly TaskService _taskService;
    
    private bool _isAddingNew;
    private TaskModel? _selectedTask;
    
    private int _newDuration;
    private string _newDescription = string.Empty;
    
    // Selection lists
    private ObservableCollection<ProjectModel> _projects = new();
    private ObservableCollection<WorkerModel> _workers = new();
    private ObservableCollection<TaskModel> _potentialPredecessors = new();

    // Selected items for Add/Edit
    private ProjectModel? _newProject;
    private WorkerModel? _newWorker;
    private TaskModel? _newPredecessor;



    public TasksViewModel(
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        IWorkerRepository workerRepository)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _workerRepository = workerRepository ?? throw new ArgumentNullException(nameof(workerRepository));
        
        _taskService = new TaskService(taskRepository);
        Tasks = new ObservableCollection<TaskModel>();
        
        AddCommand = new RelayCommand(_ => StartAdding());
        EditCommand = new RelayCommand(_ => StartEditing());
        SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
        CancelCommand = new RelayCommand(_ => CancelAdding());
        SelectCommand = new RelayCommand(t => SelectedTask = t as TaskModel);
        RecalculateTimelineCommand = new RelayCommand(
            async _ => await RecalculateTimelineAsync(),
            _ => SelectedTask != null && SelectedTask.ProjectId > 0);
        
        _ = LoadAsync();
    }

    public ObservableCollection<TaskModel> Tasks { get; }

    public ObservableCollection<ProjectModel> Projects
    {
        get => _projects;
        set => SetProperty(ref _projects, value);
    }

    public ObservableCollection<WorkerModel> Workers
    {
        get => _workers;
        set => SetProperty(ref _workers, value);
    }

    public ObservableCollection<TaskModel> PotentialPredecessors
    {
        get => _potentialPredecessors;
        set => SetProperty(ref _potentialPredecessors, value);
    }

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

    public ProjectModel? NewProject
    {
        get => _newProject;
        set
        {
            if (SetProperty(ref _newProject, value))
            {
               UpdatePredecessorsList();
            }
        }
    }

    public WorkerModel? NewWorker
    {
        get => _newWorker;
        set => SetProperty(ref _newWorker, value);
    }

    public TaskModel? NewPredecessor
    {
        get => _newPredecessor;
        set => SetProperty(ref _newPredecessor, value);
    }

    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
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

            var pData = await _projectRepository.GetAllAsync();
            Projects.Clear();
            foreach (var p in pData) Projects.Add(p);

            var wData = await _workerRepository.GetAllAsync();
            Workers.Clear();
            foreach (var w in wData) Workers.Add(w);
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show($"Fehler beim Laden: {ex.Message}");
        }
    }

    private void StartAdding()
    {
        SelectedTask = null;
        NewDuration = 0;
        NewDescription = string.Empty;
        NewProject = null;
        NewWorker = null;
        NewPredecessor = null;
        IsAddingNew = true;
        
        UpdatePredecessorsList();
    }

    private void StartEditing()
    {
        if (SelectedTask == null) return;

        NewDuration = SelectedTask.Duration;
        NewDescription = SelectedTask.Description;
        
        // Find matching objects in lists
        NewProject = Projects.FirstOrDefault(p => p.Id == SelectedTask.ProjectId);
        NewWorker = Workers.FirstOrDefault(w => w.Id == SelectedTask.WorkerId);

        // Ensure IDs are synced if they were 0 but objects existed
        if (NewProject != null) SelectedTask.ProjectId = NewProject.Id;
        if (NewWorker != null) SelectedTask.WorkerId = NewWorker.Id;
        
        // Update predecessors based on project
        UpdatePredecessorsList();
        
        NewPredecessor = PotentialPredecessors.FirstOrDefault(t => t.Id == SelectedTask.PreviousTaskId);

        // Header aktualisieren
        OnPropertyChanged(nameof(HeaderText));
        
        IsAddingNew = true;
    }

    private void UpdatePredecessorsList()
    {
        PotentialPredecessors.Clear();
        
        if (NewProject == null) return;

        // Add all tasks that belong to the selected project
        // Exclude the task currently being edited (if any) to avoid self-reference cycles (basic check)
        var filteredTasks = Tasks.Where(t => 
            (t.ProjectId == NewProject.Id || (t.Project != null && t.Project.Id == NewProject.Id)) && 
            (SelectedTask == null || t.Id != SelectedTask.Id));
            
        foreach (var task in filteredTasks)
        {
            PotentialPredecessors.Add(task);
        }
    }

    private bool CanSave() => NewDuration > 0;

    private async System.Threading.Tasks.Task SaveAsync()
    {
        try
        {
            if (SelectedTask != null)
            {
                // Update
                SelectedTask.Duration = NewDuration;
                SelectedTask.Description = NewDescription.Trim();
                SelectedTask.Project = NewProject;
                SelectedTask.ProjectId = NewProject?.Id ?? 0;
                SelectedTask.Worker = NewWorker;
                SelectedTask.WorkerId = NewWorker?.Id ?? 0;
                SelectedTask.PreviousTaskId = NewPredecessor?.Id;
                
                // If the predecessor changed, we might want to clear NextTaskId on the *old* predecessor 
                // and set it on the *new* predecessor? 
                // For now, simpler approach: The Repository Update should handle FKs. 
                // The CPM calculation service will re-derive links later anyway or the UI 
                // might need to force a recalc upon assignment.
                // Let's rely on standard saving.
                
                await _taskRepository.UpdateAsync(SelectedTask);
                
                // Refresh list or at least re-bind (perform call for side effects if any)
                await _taskRepository.GetByIdAsync(SelectedTask.Id);
                // (Optional: update the object in ObservableCollection if needed)
            }
            else
            {
                // Create
                var model = new TaskModel
                {
                    Duration = NewDuration,
                    Description = NewDescription.Trim(),
                    Project = NewProject,
                    ProjectId = NewProject?.Id ?? 0,
                    Worker = NewWorker,
                    WorkerId = NewWorker?.Id ?? 0,
                    PreviousTaskId = NewPredecessor?.Id
                };
                var saved = await _taskRepository.AddAsync(model);
                Tasks.Add(saved);
                SelectedTask = saved;
            }
            
            IsAddingNew = false;
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
