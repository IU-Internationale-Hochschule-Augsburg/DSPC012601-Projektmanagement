using System.Collections.ObjectModel;
using System.Windows;
using Projektmanagement_DesktopApp.Models;
using Projektmanagement_DesktopApp.Repositories;

namespace Projektmanagement_DesktopApp.ViewModels;

public class WorkersViewModel : ViewModelBase
{
    private readonly IWorkerRepository _workerRepository;
    private bool _isAddingNew;
    private WorkerModel? _selectedWorker;
    private string _newName = string.Empty;

    public WorkersViewModel(IWorkerRepository workerRepository)
    {
        _workerRepository = workerRepository ?? throw new ArgumentNullException(nameof(workerRepository));
        Workers = new ObservableCollection<WorkerModel>();
        
        AddCommand = new RelayCommand(_ => StartAdding());
        SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
        CancelCommand = new RelayCommand(_ => CancelAdding());
        SelectCommand = new RelayCommand(w => SelectedWorker = w as WorkerModel);
        
        _ = LoadAsync();
    }

    public ObservableCollection<WorkerModel> Workers { get; }

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

    public string HeaderText => IsAddingNew ? "Neuer Bearbeiter" : "Bearbeiter";
    public bool IsListVisible => !IsAddingNew;

    public WorkerModel? SelectedWorker
    {
        get => _selectedWorker;
        set => SetProperty(ref _selectedWorker, value);
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
            var data = await _workerRepository.GetAllAsync();
            Workers.Clear();
            foreach (var item in data) Workers.Add(item);
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
            var model = new WorkerModel { Name = NewName.Trim() };
            var saved = await _workerRepository.AddAsync(model);
            Workers.Add(saved);
            IsAddingNew = false;
            SelectedWorker = saved;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fehler beim Speichern: {ex.Message}");
        }
    }

    private void CancelAdding() => IsAddingNew = false;
}
