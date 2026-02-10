using System.ComponentModel.DataAnnotations;

namespace Projektmanagement_DesktopApp.Models;

public class TaskModel
{
    public int Id { get; set; }
    
    public int Duration { get; set; }
    
    public DateTime StartDate { get; set; } = DateTime.Now;
    
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);
    
    public int WorkerId { get; set; }
    public int ProjectId { get; set; }

    // Relationship info
    public int? PreviousTaskId { get; set; }
    public int? NextTaskId { get; set; }
    
    public DateTime CreatedAt { get; set; }

    // Hilfseigenschaft für die Anzeige
    public string DisplayName => $"Aufgabe #{Id} ({Duration}h)";
}
