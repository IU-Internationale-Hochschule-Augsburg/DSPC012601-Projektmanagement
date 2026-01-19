namespace ProjektManagement.Repositories;

public class Projekt : DataClass
{
    public string Name { get; set; } = string.Empty;

    public List<Ressource> Ressourcen { get; } = new();

    public List<Vorgang> Vorgaenge { get; } = new();
}