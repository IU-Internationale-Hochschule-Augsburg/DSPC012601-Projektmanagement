using System.ComponentModel.DataAnnotations;

namespace Projektmanagement_DesktopApp.Models;

public class WorkerModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name ist erforderlich")]
    public string Name { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
}
