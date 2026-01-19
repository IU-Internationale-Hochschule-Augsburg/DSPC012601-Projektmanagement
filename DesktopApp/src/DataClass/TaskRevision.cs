namespace Projektmanagement_DesktopApp.DataClass;

public class TaskRevision : DataClass
{
    public int taskUid { get; set; }

    public int originalUid { get; set; }
    
    public int previousTaskUid { get; set; }

    // Optional: Revisionsnummer
    public int revisionNumber { get; set; }

    // Optional: Zeitstempel der Revision
    public DateTime revisionDate { get; set; } = DateTime.UtcNow;
}