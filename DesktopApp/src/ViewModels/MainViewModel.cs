namespace Projektmanagement_DesktopApp.ViewModels;

// Projektmanagement_DesktopApp/ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private object? _currentView;

    // Primary Constructor Ersatz für Commands
    public IRelayCommand<string> NavigateCommand { get; }

    public MainViewModel()
    {
        NavigateCommand = new RelayCommand<string>(Navigate);
        // Start-Ansicht
        Navigate("Dashboard");
    }

    private void Navigate(string? destination)
    {
        // Hier instanziieren wir die ViewModels für die Pages
        CurrentView = destination switch
        {
            "Projects" => new ProjectsOverviewModel(),
            "Workers" => new WorkersOverviewModel(),
            "Dashboard" => new DashboardViewModel(),
            _ => new DashboardViewModel() // Das hier fängt alles andere ab
        };
    }
}