using System.Collections.ObjectModel;
using System.Windows;
using Projektmanagement_DesktopApp.Models;
using Projektmanagement_DesktopApp.Repositories;

namespace Projektmanagement_DesktopApp.ViewModels;

public class ProjectsViewModel : ViewModelBase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IResourceRepository _resourceRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IWorkerRepository _workerRepository;
    private bool _isAddingNewProject;
    private bool _isEditingProject;
    private ProjectModel? _selectedProject;
    private string _newProjectName = string.Empty;
    private string _newProjectDescription = string.Empty;
    private ObservableCollection<TaskModel> _selectedProjectTasks = new();
    private ObservableCollection<ResourceModel> _selectedProjectResources = new();
    public RelayCommand EditProjectCommand { get; }
    public RelayCommand CancelAddEditProjectCommand { get; }

    public ProjectsViewModel(IProjectRepository projectRepository, IResourceRepository resourceRepository,
        ITaskRepository taskRepository, IWorkerRepository workerRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _resourceRepository = resourceRepository ?? throw new ArgumentNullException(nameof(resourceRepository));
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _workerRepository = workerRepository ?? throw new ArgumentNullException(nameof(workerRepository));
        Projects = new ObservableCollection<ProjectModel>();

        AddProjectCommand = new RelayCommand(_ => StartAdding());
        SaveProjectCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
        CancelAddEditProjectCommand = new RelayCommand(_ => CancelAddEdit());
        SelectProjectCommand = new RelayCommand(p => SelectedProject = p as ProjectModel);
        EditProjectCommand = new RelayCommand(_ => StartEditing());
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
                OnPropertyChanged(nameof(IsAddingOrEditing));
            }
        }
    }

    public bool IsEditingProject
    {
        get => _isEditingProject;
        set
        {
            if (SetProperty(ref _isEditingProject, value))
            {
                OnPropertyChanged(nameof(HeaderText));
                OnPropertyChanged(nameof(IsProjectListVisible));
                OnPropertyChanged(nameof(IsAddingOrEditing));
            }
        }
    }

    public bool IsAddingOrEditing => IsAddingNewProject || IsEditingProject;

    public string HeaderText =>
        IsAddingNewProject ? "Neues Projekt" : IsEditingProject ? "Projekt bearbeiten" : "Projekte";

    public bool IsProjectListVisible => !(IsAddingOrEditing);

    public ProjectModel? SelectedProject
    {
        get => _selectedProject;
        set
        {
            SetProperty(ref _selectedProject, value);
            if (SelectedProject is not null)
            {
                _ = LoadTasksAsync();
                _ = LoadResourcesAsync();
            }
        }
    }
    
    public ObservableCollection<TaskModel> SelectedProjectTasks
    {
        get => _selectedProjectTasks;
        set
        {
            SetProperty(ref _selectedProjectTasks, value);
            OnPropertyChanged(nameof(SelectedProject));
        }
    }
    
    public ObservableCollection<ResourceModel> SelectedProjectResources
    {
        get => _selectedProjectResources;
        set
        {
            SetProperty(ref _selectedProjectResources, value);
            OnPropertyChanged(nameof(SelectedProject));
        }
    }
    
    private async Task LoadTasksAsync()
    {
        if (SelectedProject is not null)
        {
            SelectedProjectTasks = new ObservableCollection<TaskModel>(await _taskRepository.GetAllForProjectAsync(await _projectRepository.GetByIdAsync(SelectedProject.Id)));
        }
    }
    
    private async Task LoadResourcesAsync()
    {
        if (SelectedProject is not null)
        {
            SelectedProjectResources = new ObservableCollection<ResourceModel>(await _resourceRepository.GetAllForProjectAsync(await _projectRepository.GetByIdAsync(SelectedProject.Id)));
        }
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
    public RelayCommand SelectProjectCommand { get; }

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
        SelectedProject = null;
        NewProjectName = string.Empty;
        NewProjectDescription = string.Empty;
        IsAddingNewProject = true;
    }

    private void StartEditing()
    {
        IsEditingProject = true;
        if (SelectedProject == null) return;
        NewProjectName = SelectedProject.Name;
        NewProjectDescription = SelectedProject.Description;
    }

    private bool CanSave() => !string.IsNullOrWhiteSpace(NewProjectName);

    private async System.Threading.Tasks.Task SaveAsync()
    {
        try
        {
            if (IsEditingProject && SelectedProject is not null)
            {
                // Edit-Mode
                SelectedProject.Name = NewProjectName.Trim();
                SelectedProject.Description = NewProjectDescription.Trim();
                await _projectRepository.UpdateAsync(SelectedProject);
                IsEditingProject = false;
            }
            else
            {
                // Add-Mode

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
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show($"Fehler beim Speichern: {ex.Message}");
        }
    }

    private void CancelAddEdit()
    {
        IsAddingNewProject = false;
        IsEditingProject = false;
    }

    public async System.Threading.Tasks.Task DeleteSelectedProjectAsync()
    {
        if (SelectedProject != null)
        {
            try
            {
                await _projectRepository.DeleteAsync(SelectedProject.Id);
                Projects.Remove(SelectedProject);
                SelectedProject = null;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Fehler beim Löschen: {ex.Message}");
            }
        }
    }
}