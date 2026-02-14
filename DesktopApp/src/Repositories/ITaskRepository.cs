using Projektmanagement_DesktopApp.DataClass;
using Projektmanagement_DesktopApp.Models;
using Task = System.Threading.Tasks.Task;

namespace Projektmanagement_DesktopApp.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<TaskModel>> GetAllAsync();
    Task<TaskModel> AddAsync(TaskModel task);
    Task<IEnumerable<TaskModel>> getTasksByProjectId(int projectId);
    Task<TaskModel?> GetByIdAsync(int id);
    Task<TaskModel> UpdateAsync(TaskModel task);
    Task DeleteAsync(int id);
    Task<IEnumerable<TaskModel>> GetTasksByProjectIdAsync(int projectId);
    Task<IEnumerable<TaskModel>> GetAllForProjectAsync(Project project);
}
