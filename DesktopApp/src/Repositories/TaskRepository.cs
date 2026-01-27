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
            workerUid = model.WorkerId,
            projectUid = model.ProjectId
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
            WorkerId = entity.workerUid,
            ProjectId = entity.projectUid,
            CreatedAt = entity.createDate
        };
    }
}
