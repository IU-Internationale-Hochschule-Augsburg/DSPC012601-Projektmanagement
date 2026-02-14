using System.Collections.ObjectModel;
using Projektmanagement_DesktopApp.Repositories;
using Projektmanagement_DesktopApp.Models;

namespace Projektmanagement_DesktopApp.ViewModels;

public class WorkloadViewModel : ViewModelBase
{
    private readonly ITaskRepository _taskRepository;
    private DateTime _startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
    private DateTime _endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
    private ObservableCollection<WorkloadItem> _workloadStats = new();

    public WorkloadViewModel(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
        CalculateCommand = new RelayCommand(_ => LoadWorkload());
        LoadWorkload();
    }

    public DateTime StartDate
    {
        get => _startDate;
        set 
        {
            if (SetProperty(ref _startDate, value))
                LoadWorkload();
        }
    }

    public DateTime EndDate
    {
        get => _endDate;
        set 
        {
            if (SetProperty(ref _endDate, value))
                LoadWorkload();
        }
    }

    public ObservableCollection<WorkloadItem> WorkloadStats
    {
        get => _workloadStats;
        set => SetProperty(ref _workloadStats, value);
    }

    public RelayCommand CalculateCommand { get; }

    private async void LoadWorkload()
    {
        var allTasks = await _taskRepository.GetAllAsync();
        
        // Filter tasks that overlap with the selected period
        var filteredTasks = allTasks.Where(t => 
            (t.StartDate <= EndDate && t.EndDate >= StartDate) && t.Worker != null
        ).ToList();

        var stats = filteredTasks
            .GroupBy(t => t.Worker!.Name)
            .Select(g => new WorkloadItem
            {
                WorkerName = g.Key,
                TaskCount = g.Count(),
                TotalHours = g.Sum(t => t.Duration)
            })
            .OrderByDescending(s => s.TotalHours)
            .ToList();

        WorkloadStats = new ObservableCollection<WorkloadItem>(stats);
    }
}
