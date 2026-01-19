namespace ProjektManagement.Repositories;

public abstract class DataClass
{
    public int Uid { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; protected set; }

    protected DataClass()
    {
        CreateDate = DateTime.UtcNow;
        IsActive = true;
    }
}