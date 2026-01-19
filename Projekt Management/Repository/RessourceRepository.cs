namespace ProjektManagement.Repositories;

ublic class RessourceRepository : RepositoryBase<Ressource>
{
    public RessourceRepository(string filePath="./data/ressource.json") : base(filePath) { }

    public override IReadOnlyList<Ressource> GetAll()
    {
        return ReadAllFromFile();
    }

    public override Ressource? GetByUid(int uid)
    {
        var all = ReadAllFromFile();
        return all.Find(r => r.Uid == uid);
    }

    public override Ressource Insert(Ressource entity)
    {
        var all = ReadAllFromFile();
        entity.Uid = GenerateNextUid(all);
        all.Add(entity);
        WriteAllToFile(all);
        return entity;
    }

    public override Ressource Update(Ressource entity)
    {
        var all = ReadAllFromFile();
        var index = all.FindIndex(r => r.Uid == entity.Uid);

        if (index < 0)
            throw new InvalidOperationException($"Ressource with UID {entity.Uid} not found.");

        all[index] = entity;
        WriteAllToFile(all);
        return entity;
    }

    public override bool Delete(int uid)
    {
        var all = ReadAllFromFile();
        var removed = all.RemoveAll(r => r.Uid == uid) > 0;

        if (removed)
            WriteAllToFile(all);

        return removed;
    }
}
