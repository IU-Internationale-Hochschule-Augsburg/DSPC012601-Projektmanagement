using System.Windows;
using Projektmanagement_DesktopApp.Repositories;
using Projektmanagement_DesktopApp.ViewModels;

namespace Projektmanagement_DesktopApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var dbContext = App.DbContext;

        // Initialize Repositories
        var projectRepo = new ProjectRepository(dbContext);
        var resourceRepo = new ResourceRepository(dbContext);
        var taskRepo = new TaskRepository(dbContext);
        var workerRepo = new WorkerRepository(dbContext);

        // Initialize ViewModels
        var projectsVm = new ProjectsViewModel(projectRepo);
        var resourcesVm = new ResourcesViewModel(resourceRepo);
        var tasksVm = new TasksViewModel(taskRepo);
        var workersVm = new WorkersViewModel(workerRepo);

        // Set DataContext
        DataContext = new MainViewModel(projectsVm, resourcesVm, tasksVm, workersVm);
    }
}