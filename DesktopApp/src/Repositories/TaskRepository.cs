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
            description = model.Description,
            duration = model.Duration,
            startDate = model.StartDate,
            endDate = model.EndDate,
            workerUid = model.WorkerId,
            projectUid = model.ProjectId,
            previousTaskUid = model.PreviousTaskId,
            nextTaskUid = model.NextTaskId
        };

        _context.Task.Add(entity);
        await _context.SaveChangesAsync();

        model.Id = entity.id;
        model.CreatedAt = entity.createDate;
        return model;
    }

    public async Task<IEnumerable<TaskModel>> getTasksByProjectId(int projectId)
    {
        var tasks = await _context.Task
            .Where(t => t.projectUid == projectId)
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
        var entity = await _context.Task.FirstOrDefaultAsync(t => t.id == id);
        return entity == null ? null : MapToModel(entity);
    }

    public async Task<TaskModel> UpdateAsync(TaskModel task)
    {
        var entity = await _context.Task.FirstOrDefaultAsync(t => t.id == task.Id);
        if (entity == null) throw new InvalidOperationException($"Task with id {task.Id} not found");

        // Update fields
        entity.description = task.Description;
        entity.duration = task.Duration;
        entity.startDate = task.StartDate;
        entity.endDate = task.EndDate;
        entity.workerUid = task.WorkerId;
        entity.projectUid = task.ProjectId;

        // update predecessor/successor
        entity.previousTaskUid = task.PreviousTaskId;
        entity.nextTaskUid = task.NextTaskId;

        // Save
        await _context.SaveChangesAsync();

        return MapToModel(entity);
    }

    private static TaskModel MapToModel(TaskEntity entity)
    {
        return new TaskModel
        {
            Id = entity.id,
            Description = entity.description,
            Duration = entity.duration,
            StartDate = entity.startDate,
            EndDate = entity.endDate,
            WorkerId = entity.workerUid,
            ProjectId = entity.projectUid,
            PreviousTaskId = entity.previousTaskUid,
            NextTaskId = entity.nextTaskUid,
            CreatedAt = entity.createDate
        };
    }

    // Helper to map model back to entity when needed (not used currently)
    private static void MapToEntity(TaskModel model, TaskEntity entity)
    {
        entity.description = model.Description;
        entity.duration = model.Duration;
        entity.startDate = model.StartDate;
        entity.endDate = model.EndDate;
        entity.workerUid = model.WorkerId;
        entity.projectUid = model.ProjectId;
        entity.previousTaskUid = model.PreviousTaskId;
        entity.nextTaskUid = model.NextTaskId;
    }
}
