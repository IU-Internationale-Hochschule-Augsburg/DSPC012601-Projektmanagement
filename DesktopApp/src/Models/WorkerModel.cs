using System.ComponentModel.DataAnnotations;
using Projektmanagement_DesktopApp.ViewModels;

namespace Projektmanagement_DesktopApp.Models;

public class WorkerModel : ViewModelBase
{
    private string _name = string.Empty;
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name ist erforderlich")]
    public string Name 
    { 
        get => _name;
        // Wichtig! löst OnPropertyChanged aus -> aktualisiert UI direkt nach Änderung
        set => SetProperty(ref _name, value); 
    }
    
    public DateTime CreatedAt { get; set; }
}
