namespace Projektmanagement_DesktopApp.Models;

public class TimelineChangeProposal
{
    public int TaskId { get; set; }
    public string TaskDisplayName { get; set; } = string.Empty;
    public DateTime OldStartDate { get; set; }
    public DateTime OldEndDate { get; set; }
    public DateTime NewStartDate { get; set; }
    public DateTime NewEndDate { get; set; }
    public bool HasChanged => OldStartDate != NewStartDate || OldEndDate != NewEndDate;
}
