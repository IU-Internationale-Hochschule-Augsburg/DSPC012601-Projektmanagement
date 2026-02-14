namespace Projektmanagement_DesktopApp.DataClass;

public class Task : DataClass
{
    public string Description { get; set; } = string.Empty;
    
    public int Duration { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public Worker? Worker { get; set; }
    public Project? Project { get; set; }
    
    public int? PreviousTaskUid { get; set; }
    public int? NextTaskUid { get; set; }
}