namespace Projektmanagement_DesktopApp.DataClass;

public class Ressource : DataClass
{
    public string Name { get; set; } = string.Empty;

    public int Count { get; set; }
    
    public Project? Project { get; set; }
}