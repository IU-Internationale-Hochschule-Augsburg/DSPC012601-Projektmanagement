using System.ComponentModel.DataAnnotations;
using Projektmanagement_DesktopApp.ViewModels;

namespace Projektmanagement_DesktopApp.Models;

public class ProjectModel : ViewModelBase
{
    private string _name = string.Empty;
    private string _description = string.Empty;

    public int Id { get; set; }

    [Required(ErrorMessage = "Name ist erforderlich")]
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public DateTime CreatedAt { get; set; }
}