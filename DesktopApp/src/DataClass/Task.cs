namespace Projektmanagement_DesktopApp.DataClass;

public class Task : DataClass
{
    public int duration { get; set; }

    public DateTime startDate { get; set; }

    public DateTime endDate { get; set; }

    public int workerUid { get; set; }
    public int projectUid { get; set; }
}