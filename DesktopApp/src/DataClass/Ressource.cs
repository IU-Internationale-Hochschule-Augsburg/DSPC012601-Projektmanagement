namespace Projektmanagement_DesktopApp.DataClass;

public class Ressource : DataClass
{
    public string name { get; set; } = string.Empty;

    public int count { get; set; }
    
    public int projectUid { get; set; }
}