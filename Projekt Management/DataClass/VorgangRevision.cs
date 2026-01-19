namespace ProjektManagement.Repositories;

public class VorgangRevision : DataClass
{
    public int VorgangUid { get; set; }

    public int OriginalUid { get; set; }

    // Optional: Revisionsnummer
    public int RevisionNumber { get; set; }

    // Optional: Zeitstempel der Revision
    public DateTime RevisionDate { get; set; } = DateTime.UtcNow;
}