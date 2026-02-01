using System.Collections.ObjectModel;
using System.Windows;
using Projektmanagement_DesktopApp.Models;
using Projektmanagement_DesktopApp.Repositories;

namespace Projektmanagement_DesktopApp.ViewModels;

public class ResourcesViewModel : ViewModelBase
{
    private readonly IResourceRepository _resourceRepository;
    private bool _isAddingNew;
    private ResourceModel? _selectedResource;
    private string _newName = string.Empty;
    private int _newCount;

    public ResourcesViewModel(IResourceRepository resourceRepository)
    {
        _resourceRepository = resourceRepository ?? throw new ArgumentNullException(nameof(resourceRepository));
        Resources = new ObservableCollection<ResourceModel>();
        
        AddCommand = new RelayCommand(_ => StartAdding());
        SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
        CancelCommand = new RelayCommand(_ => CancelAdding());
        SelectCommand = new RelayCommand(r => SelectedResource = r as ResourceModel);
        
        _ = LoadAsync();
    }

    public ObservableCollection<ResourceModel> Resources { get; }

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

    public string HeaderText => IsAddingNew ? "Neue Ressource" : "Ressourcen";
    public bool IsListVisible => !IsAddingNew;

    public ResourceModel? SelectedResource
    {
        get => _selectedResource;
        set => SetProperty(ref _selectedResource, value);
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

    public int NewCount
    {
        get => _newCount;
        set => SetProperty(ref _newCount, value);
    }

    public RelayCommand AddCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand SelectCommand { get; }

    private async System.Threading.Tasks.Task LoadAsync()
    {
        try
        {
            var data = await _resourceRepository.GetAllAsync();
            Resources.Clear();
            foreach (var item in data) Resources.Add(item);
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show($"Fehler beim Laden: {ex.Message}");
        }
    }

    private void StartAdding()
    {
        NewName = string.Empty;
        NewCount = 0;
        IsAddingNew = true;
    }

    private bool CanSave() => !string.IsNullOrWhiteSpace(NewName);

    private async System.Threading.Tasks.Task SaveAsync()
    {
        try
        {
            var model = new ResourceModel { Name = NewName.Trim(), Count = NewCount };
            var saved = await _resourceRepository.AddAsync(model);
            Resources.Add(saved);
            IsAddingNew = false;
            SelectedResource = saved;
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show($"Fehler beim Speichern: {ex.Message}");
        }
    }

    private void CancelAdding() => IsAddingNew = false;
}
