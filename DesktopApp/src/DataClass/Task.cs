namespace Projektmanagement_DesktopApp.DataClass;

public class Task : DataClass
{
    public string description { get; set; } = string.Empty;
    
    public int duration { get; set; }

    public DateTime startDate { get; set; }

    public DateTime endDate { get; set; }

    public Worker? worker { get; set; }
    public Project? project { get; set; }
    
    public int? previousTaskUid { get; set; }
    public int? nextTaskUid { get; set; }
}