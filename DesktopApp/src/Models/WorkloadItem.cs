namespace Projektmanagement_DesktopApp.Models;

public class WorkloadItem
{
    public string WorkerName { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public int TotalHours { get; set; }
    
    // Percentage relative to a standard 40h week (optional visual aid)
    public double LoadPercentage => Math.Min(100, (TotalHours / 40.0) * 100);
    public string LoadStatus => TotalHours > 40 ? "Überlastet" : TotalHours > 30 ? "Hohe Auslastung" : "Normal";
}
