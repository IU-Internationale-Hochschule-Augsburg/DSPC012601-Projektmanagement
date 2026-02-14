using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Projektmanagement_DesktopApp.DataClass;
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
        var tasks = await _context.Task
            .Include(t => t.Project)
            .Include(t => t.Worker)
            .ToListAsync();
        return tasks.Select(MapToModel);
    }

    public async Task<TaskModel> AddAsync(TaskModel model)
    {
        var entity = new TaskEntity
        {
            Description = model.Description,
            Duration = model.Duration,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            PreviousTaskUid = model.PreviousTaskId,
            NextTaskUid = model.NextTaskId,
            Worker = model.Worker != null ? await _context.Workers.FindAsync(model.Worker.Id) : null,
            Project = model.Project != null ? await _context.Projects.FindAsync(model.Project.Id) : null
        };

        _context.Task.Add(entity);
        await _context.SaveChangesAsync();

        model.Id = entity.Id;
        model.CreatedAt = entity.CreateDate;
        return model;
    }

    public async Task<IEnumerable<TaskModel>> GetAllForProjectAsync(Project project)
    {
        var tasks = await _context.Task
            .Include(t => t.Project)
            .Include(t => t.Worker)
            .Where(t => t.Project == project)
            .ToListAsync();
        return tasks.Select(MapToModel);
    }

    public async Task<IEnumerable<TaskModel>> getTasksByProjectId(int projectId)
    {
        var tasks = await _context.Task
            .Include(t => t.Project)
            .Include(t => t.Worker)
            .Where(t => t.Project!.Id == projectId)
            .ToListAsync();
        return tasks.Select(MapToModel);
    }

    // New: async alias for clarity
    public async Task<IEnumerable<TaskModel>> GetTasksByProjectIdAsync(int projectId)
    {
        return await getTasksByProjectId(projectId);
    }

    public async Task<TaskModel?> GetByIdAsync(int id)
    {
        var entity = await _context.Task
            .Include(t => t.Project)
            .Include(t => t.Worker)
            .FirstOrDefaultAsync(t => t.Id == id);
        return entity == null ? null : MapToModel(entity);
    }

    public async Task<TaskModel> UpdateAsync(TaskModel task)
    {
        var entity = await _context.Task.FirstOrDefaultAsync(t => t.Id == task.Id);
        if (entity == null) throw new InvalidOperationException($"Task with id {task.Id} not found");

        // Update fields
        entity.Description = task.Description;
        entity.Duration = task.Duration;
        entity.StartDate = task.StartDate;
        entity.EndDate = task.EndDate;
        entity.Worker = task.Worker != null ? await _context.Workers.FindAsync(task.Worker.Id) : null;
        entity.Project = task.Project != null ? await _context.Projects.FindAsync(task.Project.Id) : null;

        // update predecessor/successor
        entity.PreviousTaskUid = task.PreviousTaskId;
        entity.NextTaskUid = task.NextTaskId;

        // Save
        await _context.SaveChangesAsync();

        return MapToModel(entity);
    }

    private static TaskModel MapToModel(TaskEntity entity)
    {
        return new TaskModel
        {
            Id = entity.Id,
            Description = entity.Description,
            Duration = entity.Duration,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Worker = entity.Worker != null ? new WorkerModel { Id = entity.Worker.Id, Name = entity.Worker.Name } : null,
            WorkerId = entity.Worker?.Id ?? 0,
            Project = entity.Project != null ? new ProjectModel { Id = entity.Project.Id, Name = entity.Project.Name } : null,
            ProjectId = entity.Project?.Id ?? 0,
            PreviousTaskId = entity.PreviousTaskUid,
            NextTaskId = entity.NextTaskUid,
            CreatedAt = entity.CreateDate
        };
    }

    // Helper to map model back to entity when needed (not used currently)

}
