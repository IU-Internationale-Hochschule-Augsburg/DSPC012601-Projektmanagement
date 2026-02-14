namespace Projektmanagement_DesktopApp.DataClass;

public abstract class DataClass
{
    public int Id { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; protected set; }

    protected DataClass()
    {
        CreateDate = DateTime.UtcNow;
        IsActive = true;
    }
}