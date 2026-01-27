using System.Collections.ObjectModel;
using System.Windows;
using Projektmanagement_DesktopApp.Models;
using Projektmanagement_DesktopApp.Repositories;

namespace Projektmanagement_DesktopApp.ViewModels;

public class ProjectsViewModel : ViewModelBase
{
    private readonly IProjectRepository _projectRepository;
    private bool _isAddingNew;
    private ProjectModel? _selectedProject;
    private string _newName = string.Empty;

    public ProjectsViewModel(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        Projects = new ObservableCollection<ProjectModel>();
        
        AddCommand = new RelayCommand(_ => StartAdding());
        SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
        CancelCommand = new RelayCommand(_ => CancelAdding());
        SelectCommand = new RelayCommand(p => SelectedProject = p as ProjectModel);
        
        _ = LoadAsync();
    }

    public ObservableCollection<ProjectModel> Projects { get; }

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

    public string HeaderText => IsAddingNew ? "Neues Projekt" : "Projekte";
    public bool IsListVisible => !IsAddingNew;

    public ProjectModel? SelectedProject
    {
        get => _selectedProject;
        set => SetProperty(ref _selectedProject, value);
    }

    public string NewName
    {
        get => _newName;
        set
        {
            if (SetProperty(ref _newName, value))
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
            var data = await _projectRepository.GetAllAsync();
            Projects.Clear();
            foreach (var item in data) Projects.Add(item);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Laden: {ex.Message}");
        }
    }

    private void StartAdding()
    {
        NewName = string.Empty;
        IsAddingNew = true;
    }

    private bool CanSave() => !string.IsNullOrWhiteSpace(NewName);

    private async System.Threading.Tasks.Task SaveAsync()
    {
        try
        {
            var model = new ProjectModel { Name = NewName.Trim() };
            var saved = await _projectRepository.AddAsync(model);
            Projects.Add(saved);
            IsAddingNew = false;
            SelectedProject = saved;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Speichern: {ex.Message}");
        }
    }

    private void CancelAdding() => IsAddingNew = false;
}
