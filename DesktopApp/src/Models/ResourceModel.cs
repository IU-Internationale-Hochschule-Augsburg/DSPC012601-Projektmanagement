using System.ComponentModel.DataAnnotations;

namespace Projektmanagement_DesktopApp.Models;

public class ResourceModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name ist erforderlich")]
    public string Name { get; set; } = string.Empty;
    
    [Range(0, int.MaxValue, ErrorMessage = "Anzahl muss positiv sein")]
    public int Count { get; set; }
    
    public int ProjectId { get; set; }
    
    public DateTime CreatedAt { get; set; }
}
