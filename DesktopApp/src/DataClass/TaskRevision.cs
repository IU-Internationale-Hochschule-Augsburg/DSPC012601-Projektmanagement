namespace Projektmanagement_DesktopApp.DataClass;

public class TaskRevision : DataClass
{
    public int TaskUid { get; set; }

    public int OriginalUid { get; set; }
    
    public int PreviousTaskUid { get; set; }

    // Optional: Revisionsnummer
    public int RevisionNumber { get; set; }

    // Optional: Zeitstempel der Revision
    public DateTime RevisionDate { get; set; } = DateTime.UtcNow;
}