using Microsoft.EntityFrameworkCore;
using Projektmanagement_DesktopApp.DataSource;
using Projektmanagement_DesktopApp.Models;
using TaskEntity = Projektmanagement_DesktopApp.DataClass.Task;

namespace Projektmanagement_DesktopApp.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly DataSourceContext _context;

    public TaskRepository(DataSourceContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<TaskModel>> GetAllAsync()
    {
        var tasks = await _context.Task.ToListAsync();
        return tasks.Select(MapToModel);
    }

    public async Task<TaskModel> AddAsync(TaskModel model)
    {
        var entity = new TaskEntity
        {
            duration = model.Duration,
            startDate = model.StartDate,
            endDate = model.EndDate,
            worker = model.Worker,
            project = model.Project
        };

        _context.Task.Add(entity);
        await _context.SaveChangesAsync();

        model.Id = entity.id;
        model.CreatedAt = entity.createDate;
        return model;
    }

    private static TaskModel MapToModel(TaskEntity entity)
    {
        return new TaskModel
        {
            Id = entity.id,
            Duration = entity.duration,
            StartDate = entity.startDate,
            EndDate = entity.endDate,
            Worker = entity.worker,
            Project = entity.project,
            CreatedAt = entity.createDate
        };
    }
}
