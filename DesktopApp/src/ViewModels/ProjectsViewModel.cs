using System.Collections.ObjectModel;
using System.Windows;
using Projektmanagement_DesktopApp.Models;
using Projektmanagement_DesktopApp.Repositories;

namespace Projektmanagement_DesktopApp.ViewModels;

public class ProjectsViewModel : ViewModelBase
{
    private readonly IProjectRepository _projectRepository;
    private bool _isAddingNewProject;
    private bool _isEditingProject;
    private ProjectModel? _selectedProject;
    private string _newProjectName = string.Empty;
    private string _newProjectDescription = string.Empty;
    public RelayCommand EditProjectCommand { get; }
    public RelayCommand CancelAddEditProjectCommand { get; }

    public ProjectsViewModel(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
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
                Console.Out.WriteLine($"Projekt {SelectedProject.Name} saved after editing");
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