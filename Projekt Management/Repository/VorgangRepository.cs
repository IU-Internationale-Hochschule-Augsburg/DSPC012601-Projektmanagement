namespace ProjektManagement.Repositories;


public class VorgangRepository : RepositoryBase<Vorgang>
{
    public VorgangRepository(string filePath="./data/vorgang.json") : base(filePath) { }

    public override IReadOnlyList<Vorgang> GetAll()
    {
        return ReadAllFromFile();
    }

    public override Vorgang? GetByUid(int uid)
    {
        var all = ReadAllFromFile();
        return all.Find(v => v.Uid == uid);
    }

    public override Vorgang Insert(Vorgang entity)
    {
        var all = ReadAllFromFile();
        entity.Uid = GenerateNextUid(all);
        all.Add(entity);
        WriteAllToFile(all);
        return entity;
    }

    public override Vorgang Update(Vorgang entity)
    {
        var all = ReadAllFromFile();
        var index = all.FindIndex(v => v.Uid == entity.Uid);

        if (index < 0)
            throw new InvalidOperationException($"Vorgang with UID {entity.Uid} not found.");

        all[index] = entity;
        WriteAllToFile(all);
        return entity;
    }

    public override bool Delete(int uid)
    {
        var all = ReadAllFromFile();
        var removed = all.RemoveAll(v => v.Uid == uid) > 0;

        if (removed)
            WriteAllToFile(all);

        return removed;
    }
}
