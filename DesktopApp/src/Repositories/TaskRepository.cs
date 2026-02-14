using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Projektmanagement_DesktopApp.DataClass;
using Projektmanagement_DesktopApp.DataSource;
using Projektmanagement_DesktopApp.Models;
using TaskEntity = Projektmanagement_DesktopApp.DataClass.Task;
using Task = System.Threading.Tasks.Task;

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
            Description = model.Description,
            Duration = model.Duration,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            PreviousTaskUid = model.PreviousTaskId,
            NextTaskUid = model.NextTaskId,
            Worker = model.Worker,
            Project = model.Project
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
            .Where(t => t.Project == project)
            .ToListAsync();
        return tasks.Select(MapToModel);
    }

    public async Task<IEnumerable<TaskModel>> getTasksByProjectId(int projectId)
    {
        var tasks = await _context.Task
            .Where(t => t.Project.Id == projectId)
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
        var entity = await _context.Task.FirstOrDefaultAsync(t => t.Id == id);
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
        entity.Worker = task.Worker;
        entity.Project = task.Project;

        // update predecessor/successor
        entity.PreviousTaskUid = task.PreviousTaskId;
        entity.NextTaskUid = task.NextTaskId;

        // Save
        await _context.SaveChangesAsync();

        return MapToModel(entity);
    }
    
    public async Task DeleteAsync(int id)
    {
        var deleted = await _context.Task
            .Where(t => t.Id == id)
            .ExecuteDeleteAsync();

        if (deleted == 0)
        {
            throw new InvalidOperationException($"Vorgang mit ID {id} nicht gefunden.");
        }
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
            Worker = entity.Worker,
            Project = entity.Project,
            PreviousTaskId = entity.PreviousTaskUid,
            NextTaskId = entity.NextTaskUid,
            CreatedAt = entity.CreateDate
        };
    }

    // Helper to map model back to entity when needed (not used currently)
    private static void MapToEntity(TaskModel model, TaskEntity entity)
    {
        entity.Description = model.Description;
        entity.Duration = model.Duration;
        entity.StartDate = model.StartDate;
        entity.EndDate = model.EndDate;
        entity.Worker = model.Worker;
        entity.Project = model.Project;
        entity.PreviousTaskUid = model.PreviousTaskId;
        entity.NextTaskUid = model.NextTaskId;
    }
}
