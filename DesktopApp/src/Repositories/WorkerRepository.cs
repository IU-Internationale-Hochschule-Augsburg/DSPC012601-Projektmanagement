using Microsoft.EntityFrameworkCore;
using Projektmanagement_DesktopApp.DataClass;
using Projektmanagement_DesktopApp.DataSource;
using Projektmanagement_DesktopApp.Models;
using Task = System.Threading.Tasks.Task;

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
            Name = model.Name
        };

        _context.Workers.Add(entity);
        await _context.SaveChangesAsync();

        model.Id = entity.Id;
        model.CreatedAt = entity.CreateDate;
        return model;
    }

    private static WorkerModel MapToModel(Worker entity)
    {
        return new WorkerModel
        {
            Id = entity.Id,
            Name = entity.Name,
            CreatedAt = entity.CreateDate
        };
    }
    
    public async Task UpdateAsync(WorkerModel model)
    {
        var entity = await _context.Workers.FindAsync(model.Id);
        if (entity != null)
        {
            entity.Name = model.Name;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var deleted = await _context.Workers
            .Where(w => w.Id == id)
            .ExecuteDeleteAsync();

        if (deleted == 0)
        {
            throw new InvalidOperationException($"Mitarbeiter mit ID {id} wurde nicht gefunden.");
        }
    }
}
