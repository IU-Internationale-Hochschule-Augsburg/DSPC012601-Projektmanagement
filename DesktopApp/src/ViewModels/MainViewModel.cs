namespace Projektmanagement_DesktopApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    private readonly ProjectsViewModel _projectsViewModel;
    private readonly ResourcesViewModel _resourcesViewModel;
    private readonly TasksViewModel _tasksViewModel;
    private readonly WorkersViewModel _workersViewModel;

    public MainViewModel(
        ProjectsViewModel projectsViewModel,
        ResourcesViewModel resourcesViewModel,
        TasksViewModel tasksViewModel,
        WorkersViewModel workersViewModel)
    {
        _projectsViewModel = projectsViewModel;
        _resourcesViewModel = resourcesViewModel;
        _tasksViewModel = tasksViewModel;
        _workersViewModel = workersViewModel;

        _currentViewModel = _projectsViewModel;

        NavigateCommand = new RelayCommand(p => Navigate(p as string));
    }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public RelayCommand NavigateCommand { get; }

    private void Navigate(string? destination)
    {
        CurrentViewModel = destination switch
        {
            "Projects" => _projectsViewModel,
            "Resources" => _resourcesViewModel,
            "Tasks" => _tasksViewModel,
            "Workers" => _workersViewModel,
            _ => _projectsViewModel
        };
        OnPropertyChanged(nameof(CurrentViewName));
    }

    public string CurrentViewName => CurrentViewModel switch
    {
        ProjectsViewModel => "Projects",
        ResourcesViewModel => "Resources",
        TasksViewModel => "Tasks",
        WorkersViewModel => "Workers",
        _ => "Projects"
    };
}
