namespace Projektmanagement_DesktopApp.DataClass;

public class Employee : DataClass
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public string FullName => $"{FirstName} {LastName}";
}