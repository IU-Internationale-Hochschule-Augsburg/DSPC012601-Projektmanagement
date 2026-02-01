using Microsoft.EntityFrameworkCore;
using Projektmanagement_DesktopApp.DataClass;
using Projektmanagement_DesktopApp.DataSource;
using Projektmanagement_DesktopApp.Models;

namespace Projektmanagement_DesktopApp.Repositories;

public class WorkerRepository : IWorkerRepository
{
    private readonly DataSourceContext _context;

    public WorkerRepository(DataSourceContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<WorkerModel>> GetAllAsync()
    {
        var workers = await _context.Workers.ToListAsync();
        return workers.Select(MapToModel);
    }

    public async Task<WorkerModel> AddAsync(WorkerModel model)
    {
        var entity = new Worker
        {
            name = model.Name
        };

        _context.Workers.Add(entity);
        await _context.SaveChangesAsync();

        model.Id = entity.id;
        model.CreatedAt = entity.createDate;
        return model;
    }

    private static WorkerModel MapToModel(Worker entity)
    {
        return new WorkerModel
        {
            Id = entity.id,
            Name = entity.name,
            CreatedAt = entity.createDate
        };
    }
}
