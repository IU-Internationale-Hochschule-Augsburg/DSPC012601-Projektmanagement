namespace ProjektManagement.Repositories;

public class VorgangRevisionRepository : RepositoryBase<VorgangRevision>
{
    public VorgangRevisionRepository(string filePath) : base(filePath) { }

    public override IReadOnlyList<VorgangRevision> GetAll()
    {
        return ReadAllFromFile();
    }

    public override VorgangRevision? GetByUid(int uid)
    {
        var all = ReadAllFromFile();
        return all.Find(r => r.Uid == uid);
    }

    public override VorgangRevision Insert(VorgangRevision entity)
    {
        var all = ReadAllFromFile();
        entity.Uid = GenerateNextUid(all);
        all.Add(entity);
        WriteAllToFile(all);
        return entity;
    }
