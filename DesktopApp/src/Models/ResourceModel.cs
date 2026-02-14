using System.ComponentModel.DataAnnotations;
using Projektmanagement_DesktopApp.ViewModels;

namespace Projektmanagement_DesktopApp.Models;

public class ResourceModel : ViewModelBase
{
    private string _name = string.Empty;
    private int _count;

    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name ist erforderlich")]
    public string Name 
    { 
        get => _name; 
        // Wichtig! löst OnPropertyChanged aus -> aktualisiert UI direkt nach Änderung
        set => SetProperty(ref _name, value); 
    }
    
    [Range(0, int.MaxValue, ErrorMessage = "Anzahl muss positiv sein")]
    public int Count 
    { 
        get => _count; 
        // Wichtig! löst OnPropertyChanged aus -> aktualisiert UI direkt nach Änderung
        set => SetProperty(ref _count, value); 
    }
    
    public int ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }
}
