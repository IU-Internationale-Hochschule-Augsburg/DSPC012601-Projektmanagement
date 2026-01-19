namespace ProjektManagement.Repositories;


public class WorkerRepository : RepositoryBase<Worker>
{
    public WorkerRepository(string filePath) : base(filePath) { }

    public override IReadOnlyList<Worker> GetAll()
    {
        return ReadAllFromFile();
    }

    public override Worker? GetByUid(int uid)
    {
        var all = ReadAllFromFile();
        return all.Find(w => w.Uid == uid);
    }

    public override Worker Insert(Worker entity)
    {
        var all = ReadAllFromFile();
        entity.Uid = GenerateNextUid(all);
        all.Add(entity);
        WriteAllToFile(all);
        return entity;
    }

    public override Worker Update(Worker entity)
    {
        var all = ReadAllFromFile();
        var index = all.FindIndex(w => w.Uid == entity.Uid);

        if (index < 0)
            throw new InvalidOperationException($"Worker with UID {entity.Uid} not found.");

        all[index] = entity;
        WriteAllToFile(all);
        return entity;
    }

    public override bool Delete(int uid)
    {
        var all = ReadAllFromFile();
        var removed = all.RemoveAll(w => w.Uid == uid) > 0;

        if (removed)
            WriteAllToFile(all);

        return removed;
    }
}
