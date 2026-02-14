using System.ComponentModel.DataAnnotations;
using Projektmanagement_DesktopApp.DataClass;

namespace Projektmanagement_DesktopApp.Models;

public class TaskModel
{
    public int Id { get; set; }
    
    public int Duration { get; set; }
    
    public DateTime StartDate { get; set; } = DateTime.Now;
    
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);
    
    public Worker? Worker { get; set; }
    public Project? Project { get; set; }
    
    public DateTime CreatedAt { get; set; }

    // Hilfseigenschaft für die Anzeige
    public string DisplayName => $"Aufgabe #{Id} ({Duration}h)";
}
