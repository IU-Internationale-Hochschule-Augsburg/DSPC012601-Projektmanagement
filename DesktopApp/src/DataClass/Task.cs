namespace Projektmanagement_DesktopApp.DataClass;

public class Task : DataClass
{
    public int duration { get; set; }

    public DateTime startDate { get; set; }

    public DateTime endDate { get; set; }

    public Worker? worker { get; set; }
    public Project? project { get; set; }
}