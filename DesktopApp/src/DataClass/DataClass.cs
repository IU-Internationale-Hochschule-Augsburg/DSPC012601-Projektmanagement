namespace Projektmanagement_DesktopApp.DataClass;

public abstract class DataClass
{
    public int id { get; set; }

    public bool isActive { get; set; }

    public DateTime createDate { get; protected set; }

    protected DataClass()
    {
        createDate = DateTime.UtcNow;
        isActive = true;
    }
}