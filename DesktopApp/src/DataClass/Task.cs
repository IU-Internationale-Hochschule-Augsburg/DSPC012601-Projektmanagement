﻿namespace Projektmanagement_DesktopApp.DataClass;

public class Task : DataClass
{
    public int duration { get; set; }

    public DateTime startDate { get; set; }

    public DateTime endDate { get; set; }

    public int workerUid { get; set; }
    public int projectUid { get; set; }
    
    // Allow null to indicate no predecessor/successor
    public int? previousTaskUid { get; set; }
    public int? nextTaskUid { get; set; }
}