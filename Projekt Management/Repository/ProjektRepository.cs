namespace ProjektManagement.Repositories;



public class ProjektRepository : RepositoryBase<Projekt>
{
    public ProjektRepository(string filePath="./data/projekte.json") : base(filePath) { }
    
    public override IReadOnlyList<Projekt> GetAll()
    {
        return ReadAllFromFile();
    }

    public override Projekt? GetByUid(int uid)
    {
        var all = ReadAllFromFile();
        return all.Find(p => p.Uid == uid);
    }

    public override Projekt Insert(Projekt entity)
    {
        var all = ReadAllFromFile();
        entity.Uid = GenerateNextUid(all);
        all.Add(entity);
        WriteAllToFile(all);
        return entity;
    }

    public override Projekt Update(Projekt entity)
    {
        var all = ReadAllFromFile();
        var index = all.FindIndex(p => p.Uid == entity.Uid);

        if (index < 0)
            throw new InvalidOperationException($"Projekt with UID {entity.Uid} not found.");

        all[index] = entity;
        WriteAllToFile(all);
        return entity;
    }

    public override bool Delete(int uid)
    {
        var all = ReadAllFromFile();
        var removed = all.RemoveAll(p => p.Uid == uid) > 0;

        if (removed)
            WriteAllToFile(all);

        return removed;
    }
}