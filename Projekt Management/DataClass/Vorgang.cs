namespace ProjektManagement.Repositories;

public class Vorgang : DataClass
{
    public int Dauer { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int WorkerUid { get; set; }
}