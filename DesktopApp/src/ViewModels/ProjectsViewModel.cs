using System.Collections.ObjectModel;
using System.Windows;
using Projektmanagement_DesktopApp.Models;
using Projektmanagement_DesktopApp.Repositories;
using Projektmanagement_DesktopApp.Services;

namespace Projektmanagement_DesktopApp.ViewModels;

public class ProjectsViewModel : ViewModelBase
{
    private readonly IProjectRepository _projectRepository;
    private bool _isAddingNewProject;
    private ProjectModel? _selectedProject;
    private string _newProjectName = string.Empty;
    private string _newProjectDescription = string.Empty;

    public ProjectsViewModel(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        Projects = new ObservableCollection<ProjectModel>();
        
        AddProjectCommand = new RelayCommand(_ => StartAdding());
        SaveProjectCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
        CancelAddProjectCommand = new RelayCommand(_ => CancelAdding());
        SelectProjectCommand = new RelayCommand(p => SelectedProject = p as ProjectModel);
        UpdateTimelineCommand = new RelayCommand(async _ => await UpdateTimelineAsync(), _ => SelectedProject != null);
        
        _ = LoadAsync();
    }

    public ObservableCollection<ProjectModel> Projects { get; }

    public bool IsAddingNewProject
    {
        get => _isAddingNewProject;
        set
        {
            if (SetProperty(ref _isAddingNewProject, value))
            {
                OnPropertyChanged(nameof(HeaderText));
                OnPropertyChanged(nameof(IsProjectListVisible));
            }
        }
    }

    public string HeaderText => IsAddingNewProject ? "Neues Projekt" : "Projekte";
    public bool IsProjectListVisible => !IsAddingNewProject;

    public ProjectModel? SelectedProject
    {
        get => _selectedProject;
        set => SetProperty(ref _selectedProject, value);
    }

    public string NewProjectName
    {
        get => _newProjectName;
        set
        {
            if (SetProperty(ref _newProjectName, value))
            {
                SaveProjectCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string NewProjectDescription
    {
        get => _newProjectDescription;
        set => SetProperty(ref _newProjectDescription, value);
    }

    public RelayCommand AddProjectCommand { get; }
    public RelayCommand SaveProjectCommand { get; }
    public RelayCommand CancelAddProjectCommand { get; }
    public RelayCommand SelectProjectCommand { get; }
    public RelayCommand UpdateTimelineCommand { get; }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        try
        {
            var data = await _projectRepository.GetAllAsync();
            Projects.Clear();
            foreach (var item in data) Projects.Add(item);
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show($"Fehler beim Laden: {ex.Message}");
        }
    }

    private void StartAdding()
    {
        NewProjectName = string.Empty;
        NewProjectDescription = string.Empty;
        IsAddingNewProject = true;
    }

    private bool CanSave() => !string.IsNullOrWhiteSpace(NewProjectName);

    private async System.Threading.Tasks.Task SaveAsync()
    {
        try
        {
            var model = new ProjectModel 
            { 
                Name = NewProjectName.Trim(),
                Description = NewProjectDescription.Trim()
            };
            var saved = await _projectRepository.AddAsync(model);
            Projects.Add(saved);
            IsAddingNewProject = false;
            SelectedProject = saved;
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show($"Fehler beim Speichern: {ex.Message}");
        }
    }

    private void CancelAdding() => IsAddingNewProject = false;

    private async System.Threading.Tasks.Task UpdateTimelineAsync()
    {
        if (SelectedProject == null)
        {
            MessageBox.Show("Bitte wählen Sie ein Projekt aus.");
            return;
        }

        try
        {
            var dbContext = App.DbContext;
            var taskService = new TaskService(new TaskRepository(dbContext));
            await taskService.RecalculateProjectTimelineAsync(SelectedProject.Id, true);
            MessageBox.Show("Timeline erfolgreich aktualisiert.");
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show($"Fehler beim Aktualisieren der Timeline: {ex.Message}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unerwarteter Fehler: {ex.Message}");
        }
    }
}
