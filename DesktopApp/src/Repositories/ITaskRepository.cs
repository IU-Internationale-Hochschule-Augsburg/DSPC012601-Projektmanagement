using Projektmanagement_DesktopApp.Models;

namespace Projektmanagement_DesktopApp.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<TaskModel>> GetAllAsync();
    Task<TaskModel> AddAsync(TaskModel task);
    Task<IEnumerable<TaskModel>> getTasksByProjectId(int projectId);
    Task<TaskModel?> GetByIdAsync(int id);
    Task<TaskModel> UpdateAsync(TaskModel task);
    Task<IEnumerable<TaskModel>> GetTasksByProjectIdAsync(int projectId);
}
