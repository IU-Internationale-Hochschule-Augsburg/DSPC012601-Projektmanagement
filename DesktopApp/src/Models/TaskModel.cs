using System.ComponentModel.DataAnnotations;
using Projektmanagement_DesktopApp.DataClass;

namespace Projektmanagement_DesktopApp.Models;

public class TaskModel
{
    public int Id { get; set; }
    
    public string Description { get; set; } = string.Empty;
    
    public int Duration { get; set; }
    
    public DateTime StartDate { get; set; } = DateTime.Now;
    
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);
    
    public WorkerModel? Worker { get; set; }
    public ProjectModel? Project { get; set; }
    public int WorkerId { get; set; }
    public int ProjectId { get; set; }

    // Relationship info
    public int? PreviousTaskId { get; set; }
    public int? NextTaskId { get; set; }
    
    public DateTime CreatedAt { get; set; }

    // Hilfseigenschaft für die Anzeige
    public string DisplayName => string.IsNullOrWhiteSpace(Description)
        ? $"Aufgabe #{Id} ({Duration}h)"
        : $"{Description} ({Duration}h)";
}
