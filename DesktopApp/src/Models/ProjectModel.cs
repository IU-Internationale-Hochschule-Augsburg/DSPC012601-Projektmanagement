using System.ComponentModel.DataAnnotations;

namespace Projektmanagement_DesktopApp.Models;

public class ProjectModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name ist erforderlich")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
